# DIMS

Monorepo del proyecto DIMS con tres piezas:

- [DIMS-Backend](/mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend): API REST en .NET 10 con PostgreSQL.
- [DIMS_Astro](/mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS_Astro): frontend en Astro 5 + React 19.
- [serverless](/mnt/c/Users/dikyd/Documents/PROYECTOS/serverless): funciones AWS para compresión de imágenes y notificación por suscripción.

Este README está orientado a ejecución: dependencias, variables, arranque local, Docker y AWS.

**Arquitectura**

```text
Astro frontend -> .NET backend -> PostgreSQL
                           -> S3 (uploads y payloads de suscripción)
S3 ObjectCreated -> Lambdas serverless -> SES / SNS
```

**Dependencias**

Para ejecutar el repo completo necesitas:

- `Docker` y `Docker Compose` para el arranque más simple del stack principal.
- `Node.js 20.x` y `npm` para frontend y Lambdas.
- `.NET SDK 10.0` para backend y tests fuera de Docker.
- `PostgreSQL 17` solo si no usarás Docker para la base de datos.
- Cuenta AWS con acceso a `S3`, `Lambda`, `SES`, `SNS` si quieres habilitar el flujo completo de archivos e emails.
- `zip` o equivalente para empaquetar Lambdas manualmente.

**Versiones detectadas en el repo**

- Backend: `net10.0` en [DIMS-Backend.csproj](/mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/DIMS-Backend.csproj).
- Frontend Docker: `node:20-alpine` en [dockerfile](/mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS_Astro/dockerfile).
- Base de datos en Docker: `postgres:17-alpine` en [docker-compose.yml](/mnt/c/Users/dikyd/Documents/PROYECTOS/docker-compose.yml).
- Lambda `dims-subscription-notifier`: `Node.js 20.x` y dependencias `@aws-sdk/client-s3`, `@aws-sdk/client-sesv2`, `@aws-sdk/client-sns` en [package.json](/mnt/c/Users/dikyd/Documents/PROYECTOS/serverless/dims-subscription-notifier/package.json).

**Variables de entorno**

Copia [.env.example](/mnt/c/Users/dikyd/Documents/PROYECTOS/.env.example) a `.env` en la raíz y completa al menos estas variables:

```env
DB_HOST=postgres
DB_PORT=5432
DB_NAME=DIMS
DB_USERNAME=postgres
DB_PASSWORD=your_secure_password_here

JWT_SECRET_KEY=replace_with_a_long_random_secret
JWT_ISSUER=DIMS_Backend
JWT_AUDIENCE=DIMS_Frontend

CORS_ORIGINS=http://localhost:3000,http://127.0.0.1:3000,http://localhost:4321

ASPNETCORE_ENVIRONMENT=Production

PUBLIC_API_URL=http://localhost:8000/api
PUBLIC_APP_NAME=DIMS Portal
```

Variables opcionales o necesarias solo para AWS:

```env
AWS_ACCESS_KEY_ID=
AWS_SECRET_ACCESS_KEY=
AWS_SESSION_TOKEN=
AWS_REGION=us-east-1
S3_BUCKET_NAME=
AWS_BUCKET_NAME=
```

Notas importantes:

- El backend lee `.env` automáticamente en local cuando se ejecuta fuera de Docker desde la raíz o con esas variables exportadas.
- El frontend falla al iniciar si falta `PUBLIC_API_URL`; en SSR con Docker también usa `INTERNAL_API_URL`.
- El backend puede iniciar sin credenciales AWS, pero las rutas o procesos que suben a S3 fallarán si `S3_BUCKET_NAME` o las credenciales no están configuradas.

**Ejecución recomendada con Docker**

1. Crea `.env` en la raíz a partir de [.env.example](/mnt/c/Users/dikyd/Documents/PROYECTOS/.env.example).
2. Ajusta como mínimo `DB_PASSWORD`, `JWT_SECRET_KEY` y `PUBLIC_API_URL=http://localhost:8000/api`.
3. Levanta el stack:

```bash
docker compose up --build
```

Servicios expuestos:

- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:8000`
- Scalar/OpenAPI: `http://localhost:8000/scalar/v1`
- Health check: `http://localhost:8000/health`
- PostgreSQL host: `localhost:5433` si mantienes el mapeo por defecto del compose

Qué levanta [docker-compose.yml](/mnt/c/Users/dikyd/Documents/PROYECTOS/docker-compose.yml):

- `postgres`
- `backend`
- `frontend`

Para un despliegue más cercano a producción existe [docker-compose.prod.yml](/mnt/c/Users/dikyd/Documents/PROYECTOS/docker-compose.prod.yml), que además agrega `nginx` y espera imágenes publicadas en GHCR.

**Ejecución sin Docker**

Base de datos:

1. Levanta PostgreSQL localmente.
2. Crea la base `DIMS`.
3. Ajusta `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USERNAME`, `DB_PASSWORD`.

Backend:

```bash
dotnet restore /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/DIMS-Backend.csproj
dotnet run --project /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/DIMS-Backend.csproj
```

Frontend:

```bash
cd /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS_Astro
npm install
PUBLIC_API_URL=http://localhost:8000/api npm run dev
```

El frontend en desarrollo usa Astro y normalmente queda disponible en `http://localhost:4321`.

**Tests**

Backend:

```bash
dotnet test /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/tests/DIMS-Backend.Tests.csproj
dotnet test /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/tests/DIMS-Backend.Tests.csproj --filter "FullyQualifiedName~DIMS_Backend.Tests.Unit"
dotnet test /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS-Backend/tests/DIMS-Backend.Tests.csproj --filter "FullyQualifiedName~DIMS_Backend.Tests.Behavior"
```

Frontend:

```bash
cd /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS_Astro
npm install
npm run test:unit
npm run test:behavior
npm run test:ci
```

Comandos útiles del frontend:

```bash
cd /mnt/c/Users/dikyd/Documents/PROYECTOS/DIMS_Astro
npm run validate
npm run build
```

**AWS y flujo completo**

Si quieres que el repo completo funcione con uploads, procesamiento en S3 y emails, además del stack principal debes configurar AWS.

Backend:

- Usa `S3_BUCKET_NAME` para subir archivos e insertar payloads de suscripción en S3.
- Requiere credenciales con permisos de escritura al bucket.

Serverless:

- [serverless/dims-image-compressor](/mnt/c/Users/dikyd/Documents/PROYECTOS/serverless/dims-image-compressor) procesa imágenes subidas a S3.
- [serverless/dims-subscription-notifier](/mnt/c/Users/dikyd/Documents/PROYECTOS/serverless/dims-subscription-notifier) lee JSONs bajo `subscriptions/` y envía correos por SES.

Para `dims-subscription-notifier` necesitas:

- Runtime `Node.js 20.x`
- `Handler`: `index.handler`
- Variables:
  - `AWS_REGION=us-east-1`
  - `SES_FROM_EMAIL=<correo verificado en SES>`
  - `SES_REPLY_TO=<opcional>`
  - `SNS_TOPIC_ARN=<opcional>`
- Permisos IAM:
  - `s3:GetObject`
  - `ses:SendEmail`
  - `logs:CreateLogGroup`
  - `logs:CreateLogStream`
  - `logs:PutLogEvents`
- Trigger S3:
  - Event type `ObjectCreated`
  - Prefix `subscriptions/`

Empaquetado manual de `dims-subscription-notifier`:

```bash
cd /mnt/c/Users/dikyd/Documents/PROYECTOS/serverless/dims-subscription-notifier
npm install
zip -r function.zip index.js package.json node_modules
```

Luego sube `function.zip` a la Lambda.

Limitación de SES:

- Si la cuenta está en sandbox, solo podrás enviar a destinatarios verificados.

Más detalle de la parte serverless en [serverless/README.md](/mnt/c/Users/dikyd/Documents/PROYECTOS/serverless/README.md).

**Problemas comunes**

- `Missing API URL`: falta `PUBLIC_API_URL` o `INTERNAL_API_URL` en SSR.
- Error de conexión a Postgres: revisa `DB_HOST`, `DB_PORT` y que el contenedor `postgres` esté sano.
- Uploads o suscripciones no llegan a AWS: faltan credenciales, bucket o permisos IAM.
- La Lambda existe pero no hace nada: normalmente falta el trigger S3, variables de entorno o subiste el código sin `node_modules`.
