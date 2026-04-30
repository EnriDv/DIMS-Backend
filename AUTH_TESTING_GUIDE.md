# Testing Guide: Auth System with Refresh & Logout

## Requisitos Previos
- Backend ejecutando en `http://localhost:5231/api`
- PostgreSQL conectado con la tabla `users` creada
- Un usuario admin creado

## 1. Crear Usuario Admin

```bash
POST /api/Auth/seed-admin
Content-Type: application/json

{
  "nombre": "Admin Test",
  "email": "admin@ucb.edu.bo",
  "password": "Passw0rd123",
  "rol": "admin"
}

Respuesta (200):
{
  "mensaje": "Usuario administrador creado con éxito",
  "userId": "uuid-aqui"
}
```

## 2. Login - Obtener Access Token + Refresh Token

```bash
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@ucb.edu.bo",
  "password": "Passw0rd123"
}

Respuesta (200):
{
  "accessToken": "eyJhbGc...",  // 15 minutos
  "refreshToken": "eyJhbGc...", // 7 días
  "userId": "uuid",
  "nombre": "Admin Test",
  "email": "admin@ucb.edu.bo",
  "rol": "admin"
}
```

**Guardar ambos tokens** en localStorage (frontend lo hace automáticamente)

## 3. Acceder a Recurso Protegido

```bash
GET /api/Noticias/admin
Authorization: Bearer {accessToken}

Respuesta (200):
[Listado de noticias]
```

## 4. Simular Expiración del Access Token

**Opción A: Esperar 15 minutos** (no es práctico para testing)

**Opción B: Borrar el accessToken manualmente en devtools**
```javascript
// En console del navegador:
localStorage.removeItem('accessToken')
```

Ahora intenta hacer cualquier request protegido → recibirás **401 Unauthorized**

## 5. Refrescar Tokens - Token Rotation

El cliente hace esto automáticamente cuando recibe 401:

```bash
POST /api/Auth/refresh
Content-Type: application/json

{
  "refreshToken": "{refreshToken guardado}"
}

Respuesta (200):
{
  "accessToken": "eyJhbGc...",  // Nuevo, 15 min
  "refreshToken": "eyJhbGc..."  // Nuevo, 7 días
}
```

**El cliente actualiza automáticamente localStorage** y reintenta la request original.

## 6. Logout - Invalidar Sesión

```bash
POST /api/Auth/logout
Authorization: Bearer {accessToken}

Respuesta (200):
{
  "message": "Sesión cerrada correctamente. Los tokens han sido invalidados."
}
```

El **cliente limpia localStorage**:
- `accessToken` eliminado
- `refreshToken` eliminado  
- `currentUser` eliminado

## 7. Después del Logout

Si intentas acceder a recurso protegido sin tokens:

```bash
GET /api/Noticias/admin
Authorization: (vacío)

Respuesta (401):
{
  "title": "No autorizado",
  "detail": "..."
}
```

Frontend redirige a `/login?error=unauthorized`

---

## Frontend Testing (Astro)

### Test Flow Completo

1. **Ir a `/login`**
2. **Ingresar credenciales:**
   - Email: `admin@ucb.edu.bo`
   - Password: `Passw0rd123`
3. **Login exitoso:**
   - Se guardan `accessToken`, `refreshToken`, `currentUser` en localStorage
   - Redirect a `/` (home)
   - En header aparece nombre del usuario

4. **Ir a `/admin` (ruta protegida)**
   - Se renderiza AdminPanel
   - Puedes crear eventos/noticias

5. **Simular expiración de access token:**
   - En DevTools > Application > localStorage
   - Busca `accessToken` y cópialo
   - Elimina `accessToken`
   - Intenta crear un evento
   - Cliente detecta 401 → auto-refresh con refreshToken
   - Nuevo accessToken en localStorage
   - Request reintentada → exitosa

6. **Logout:**
   - Click en botón Logout (en Header o AdminPanel)
   - Se llama `POST /api/Auth/logout`
   - localStorage limpiado
   - Redirect a `/login`

---

## Casos de Error

### Error 1: Refresh Token Expirado (7 días)
```
POST /api/Auth/refresh
{ "refreshToken": "token_expirado" }

Respuesta (401):
{
  "title": "No autorizado",
  "detail": "Refresh token inválido o expirado."
}

→ Cliente hace logout automático
→ Redirige a /login
```

### Error 2: Credenciales Inválidas
```
POST /api/Auth/login
{ "email": "wrong@ucb.edu.bo", "password": "wrong" }

Respuesta (401):
{
  "title": "Credenciales incorrectas o cuenta inactiva."
}
```

### Error 3: Usuario Inactivo
```
POST /api/Auth/login
{ "email": "admin@ucb.edu.bo", "password": "..." }

(Si user.Activo = false)

Respuesta (401):
{
  "title": "Credenciales incorrectas o cuenta inactiva."
}
```

---

## Verificación en Base de Datos

```sql
-- Ver usuario creado
SELECT id, email, nombre, rol, activo FROM users 
WHERE email = 'admin@ucb.edu.bo';

-- Ver hash de password (nunca password en texto plano)
SELECT password_hash FROM users 
WHERE email = 'admin@ucb.edu.bo';
```

**El hash de `Passw0rd123` comienza con `$2a$` (formato bcrypt)**

---

## Debugging Tips

### Decodificar JWT (sin validar firma)
Usa https://jwt.io/

```
accessToken: {
  "sub": "user-id",
  "email": "admin@ucb.edu.bo",
  "nombre": "Admin Test",
  "role": "admin",
  "exp": 1234567890,  // Expira en 15 min
  "iat": 1234567200,
  "iss": "DIMS_Backend",
  "aud": "DIMS_Frontend"
}

refreshToken: {
  "sub": "user-id",
  "email": "admin@ucb.edu.bo",
  "nombre": "Admin Test",
  "role": "admin",
  "exp": 9999999999,  // Expira en 7 días
  ...
}
```

### DevTools Network Tab
1. Abre Network en F12
2. Haz login
3. Busca `login` → response tiene ambos tokens
4. Haz request a `/api/Noticias/admin` → headers tiene `Authorization: Bearer {accessToken}`
5. Elimina accessToken y reintenta
6. Busca `refresh` → cliente lo hizo automáticamente
7. Nuevo `Authorization` con nuevo accessToken

### Logs del Backend
```
dotnet run
// Verás logs de validación JWT cuando hagas requests

// Si refresh falla:
Unauthorized: Refresh token inválido o expirado.
```

---

## Flujo Completo en Timeline

```
0:00 - User clic Login
0:05 - POST /Auth/login → response con access + refresh tokens
0:10 - User navega a /admin (protected)
0:15 - GET /api/Noticias/admin con accessToken → 200 OK
0:20 - User intenta crear evento
15:00 - Access token expira
15:01 - POST /api/Noticias (create) → 401 Unauthorized
15:02 - Cliente auto-refresh: POST /Auth/refresh
15:03 - Nuevos tokens recibidos
15:04 - POST /api/Noticias reintentada con nuevo accessToken → 200 OK
15:10 - User clic Logout
15:11 - POST /api/Auth/logout → success
15:12 - localStorage limpiado
15:13 - Redirect a /login
```

---

## Próximos Pasos (Opcionales)

1. **Agregar blacklist de tokens** (tabla RefreshTokens) si quieres logout instantáneo
2. **Agregar 2FA** (two-factor authentication)
3. **Agregar refresh token rotation**: cada refresh genera tokens completamente nuevos
4. **Agregar CSRF protection** si usas cookies en lugar de localStorage
5. **Agregar logging/auditing** de accesos y logouts
