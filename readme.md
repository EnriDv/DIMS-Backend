***

# 🚀 API Documentation - DIMS Backend (Portal de Ingenierías UCB)

Esta API está construida con **.NET 10** usando arquitectura **CQRS (MediatR)** y **PostgreSQL**. La mayoría de las rutas de mutación (POST, PUT, DELETE) requieren autenticación mediante **JWT** (Token Bearer).

---

## 🔐 1. Módulo de Autenticación (`/api/Auth`)

### `POST /api/Auth/login`
* [cite_start]**Descripción:** Inicia sesión y genera un token JWT para acceder a rutas protegidas[cite: 3, 4].
* **Recibe (Body):**
    ```json
    {
      "email": "usuario@ucb.edu.bo",
      "password": "MiPassword123"
    }
    ```
* **Devuelve (200 OK):**
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5c...",
      "userId": "uuid-del-usuario",
      "nombre": "Enrique Diaz",
      "rol": "admin"
    }
    ```

### `POST /api/Auth/seed-admin`
* [cite_start]**Descripción:** Crea un usuario administrador inicial en el sistema[cite: 4].
* **Recibe (Body):**
    ```json
    {
      "nombre": "Admin Principal",
      "email": "admin@ucb.edu.bo",
      "password": "Password123!",
      "rol": "admin"
    }
    ```
* **Devuelve (200 OK):** Mensaje de confirmación o el ID del usuario creado.

---

## 🎓 2. Módulo de Carreras (`/api/Carreras`)

### `GET /api/Carreras`
* [cite_start]**Descripción:** Obtiene la lista de todas las carreras activas[cite: 3].
* **Recibe:** Nada.
* **Devuelve (200 OK):** Un arreglo de objetos `CarreraListDto` (id, nombre, slug, modalidad, duracion, icono, color).

### `GET /api/Carreras/{id}`
* [cite_start]**Descripción:** Obtiene los detalles completos de una carrera específica[cite: 3].
* **Recibe (Path):** `id` (entero).
* **Devuelve (200 OK):** Objeto `CarreraDetailDto` que incluye la lista de "Perfil de Egresado" y "Campo Laboral".

### `GET /api/Carreras/{id}/malla`
* [cite_start]**Descripción:** Obtiene la malla curricular de una carrera, ordenada por semestre[cite: 3].
* **Recibe (Path):** `id` (entero).
* **Devuelve (200 OK):** Un arreglo de objetos `MateriaMallaDto`.

### `POST /api/Carreras`
* [cite_start]**Descripción:** Crea una nueva carrera (Requiere rol `admin`)[cite: 3].
* **Recibe (Body):**
    ```json
    {
      "nombre": "Ingeniería de Software",
      "slug": "ingenieria-software",
      "descripcion": "Descripción de la carrera...",
      "duracion": "8 Semestres",
      "modalidad": "Presencial",
      "color": "#3B82F6",
      "icono": "💻"
    }
    ```
* **Devuelve (201 Created):** El `id` de la carrera recién creada.

### `PUT /api/Carreras/{id}`
* [cite_start]**Descripción:** Actualiza los datos principales de una carrera (Requiere rol `admin`)[cite: 3].
* [cite_start]**Recibe (Path & Body):** `id` en la URL y el siguiente JSON[cite: 3]:
    ```json
    {
      "id": 1,
      "nombre": "Nuevo Nombre",
      "descripcion": "Nueva descripción",
      "activa": true
    }
    ```
* **Devuelve (204 No Content):** Vacío si fue exitoso.

### `DELETE /api/Carreras/{id}`
* [cite_start]**Descripción:** Realiza un borrado lógico de una carrera (Requiere rol `admin`)[cite: 3].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 3].
* **Devuelve (204 No Content):** Vacío si fue exitoso.

---

## 📚 3. Módulo de Materias (`/api/Materias`)

### `GET /api/Materias/{id}`
* [cite_start]**Descripción:** Obtiene el detalle de una materia, incluyendo docentes que la imparten y paralelos disponibles[cite: 2].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 2].
* **Devuelve (200 OK):** Objeto `MateriaDetailDto` con arrays de `Docentes` y `Paralelos`.

### `POST /api/Materias`
* [cite_start]**Descripción:** Crea una nueva materia y la asigna a una carrera (Requiere rol `admin`)[cite: 2, 3].
* **Recibe (Body):**
    ```json
    {
      "sigla": "SIS-101",
      "nombre": "Introducción a la Programación",
      "creditos": 5,
      "carreraId": 1,
      "semestre": 1,
      "tipo": "obligatoria",
      "area": "Ciencias Exactas"
    }
    ```
* **Devuelve (200 OK):** El `id` de la materia creada.

### `DELETE /api/Materias/{id}`
* [cite_start]**Descripción:** Realiza un borrado lógico de una materia (Requiere rol `admin`)[cite: 2].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 2].
* **Devuelve (204 No Content):** Vacío si fue exitoso.

---

## 👤 4. Módulo de Personas (`/api/Personas`)

### `GET /api/Personas`
* [cite_start]**Descripción:** Lista el staff docente y administrativo[cite: 1].
* **Recibe (Query Opcional):** `?carreraId=X` para filtrar.
* **Devuelve (200 OK):** Arreglo de `PersonaListDto`.

### `GET /api/Personas/{id}`
* [cite_start]**Descripción:** Perfil detallado de un docente/director, extrayendo dinámicamente sus áreas de especialidad[cite: 2].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 2].
* **Devuelve (200 OK):** Objeto `PersonaDetailDto` con array de `Materias` y `Areas`.

### `POST /api/Personas`
* [cite_start]**Descripción:** Registra un nuevo miembro del staff (Requiere rol `admin`)[cite: 1, 2].
* **Recibe (Body):**
    ```json
    {
      "nombre": "Juan Perez",
      "email": "juan@ucb.edu.bo",
      "rol": "docente",
      "carreraId": 1,
      "especialidad": "Inteligencia Artificial",
      "gradoAcademico": "PhD"
    }
    ```
* **Devuelve (200 OK):** El `id` de la persona creada.

---

## 📰 5. Módulo de Noticias (`/api/Noticias`)

### `GET /api/Noticias`
* [cite_start]**Descripción:** Obtiene el feed de noticias publicadas[cite: 2].
* **Recibe (Query Opcional):** `?carreraId=X` para filtrar.
* **Devuelve (200 OK):** Arreglo de `NoticiaDto`.

### `POST /api/Noticias`
* [cite_start]**Descripción:** Crea una noticia (Requiere rol `admin` o `docente`)[cite: 2].
* **Recibe (Body):**
    ```json
    {
      "titulo": "Nueva Noticia",
      "contenido": "<p>Contenido en HTML</p>",
      "imagenUrl": "https://link.com/img.jpg",
      "carreraId": null,
      "destacada": true
    }
    ```
* **Devuelve (200 OK / 201 Created):** El `id` de la noticia.

### `DELETE /api/Noticias/{id}`
* **Descripción:** Borrado lógico de una noticia. [cite_start]El backend verifica mediante el JWT si el usuario es el creador de la noticia o un admin[cite: 2].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 2].
* **Devuelve (204 No Content):** Vacío si fue exitoso.

---

## 📅 6. Módulo de Eventos (`/api/Eventos`)

### `GET /api/Eventos`
* [cite_start]**Descripción:** Lista los eventos próximos[cite: 3].
* **Recibe:** Nada.
* **Devuelve (200 OK):** Arreglo de `EventoListDto`.

### `POST /api/Eventos/{id}/suscribir`
* **Descripción:** Inscribe al usuario autenticado en un evento. [cite_start]Extrae el ID del estudiante directamente del Token JWT enviado en los headers[cite: 3].
* [cite_start]**Recibe (Path & Headers):** `id` del evento [cite: 3] y Header `Authorization: Bearer <token>`.
* **Devuelve (200 OK):** Mensaje de confirmación (Falla si no hay cupos).

---

## 📝 7. Módulo de Publicaciones (`/api/Publicaciones`)

### `GET /api/Publicaciones`
* [cite_start]**Descripción:** Repositorio de proyectos de grado, tesis y artículos[cite: 1].
* **Recibe (Query Opcional):** `?carreraId=X` y `?tipo=Y` (ej: tesis, proyecto).
* **Devuelve (200 OK):** Arreglo de `PublicacionDto`.

### `POST /api/Publicaciones`
* [cite_start]**Descripción:** Registra una nueva publicación académica (Requiere rol `admin` o `docente`)[cite: 1].
* **Recibe (Body):**
    ```json
    {
      "titulo": "Título de la Tesis",
      "autor": "Nombre Estudiante",
      "resumen": "Abstract...",
      "archivoUrl": "https://link-al-pdf.com",
      "carreraId": 1,
      "tipo": "tesis"
    }
    ```
* **Devuelve (200 OK):** El `id` de la publicación creada.

### `PUT /api/Publicaciones/{id}`
* **Descripción:** Edita una publicación. [cite_start]El backend inyecta y valida internamente los campos `requestUserId` y `requestUserRole` usando los claims del JWT para asegurar que solo el dueño o un admin puedan editar[cite: 1].
* [cite_start]**Recibe (Path & Body):** `id` en la URL y los campos a actualizar en el JSON[cite: 1]. *(Nota: No envíes `requestUserId` ni `requestUserRole` desde el frontend, el backend los lee de tu token de sesión)*.
* **Devuelve (204 No Content):** Vacío si fue exitoso.

### `DELETE /api/Publicaciones/{id}`
* [cite_start]**Descripción:** Realiza un borrado lógico de la publicación (Requiere ser el dueño o Admin)[cite: 1].
* [cite_start]**Recibe (Path):** `id` (entero)[cite: 1].
* **Devuelve (204 No Content):** Vacío si fue exitoso.

***
