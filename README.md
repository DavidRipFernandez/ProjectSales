# ProjectSales - ejecución local

Este repositorio contiene la solución solicitada en la Parte 2: API ASP.NET Core con autenticación JWT/Refresh, RBAC y catálogos, además de la aplicación React con Vite y Tailwind para los CRUD principales.

## Requisitos previos

- .NET SDK 8.0
- Node.js 18+ y npm
- SQL Server accesible mediante la cadena `ConnectionStrings:SqlServer`

## Backend (projectSales.Server)

1. Ajusta `appsettings.Development.json` con la cadena de conexión y verifica la sección `Jwt` y `Seed:RunOnStartup`.
2. Ejecuta las migraciones o asegúrate de que la base de datos existe según el esquema ya provisto en la Parte 1.
3. Inicia la API:
   ```bash
   dotnet run --project projectSales.Server
   ```
4. La API quedará disponible en `https://localhost:7030` y Swagger en `https://localhost:7030/swagger`.
5. En modo Development, si `Seed:RunOnStartup=true`, se insertan roles, usuarios, módulos, catálogos y la información base solicitada. También se crean la vista `dbo.vw_ActiveSessions` y el procedimiento `dbo.usp_PurgeAuthArtifacts` si no existen.

## Frontend (projectsales.client)

1. Instala dependencias:
   ```bash
   cd projectsales.client
   npm install
   ```
2. Levanta la aplicación con Vite:
   ```bash
   npm run dev
   ```
3. La interfaz se sirve en `http://localhost:5173` y consume la API con `withCredentials` para manejar el refresh token HttpOnly.
4. La UI incluye login, navegación protegida, interceptores de Axios con reintentos automáticos tras 401, gestión de permisos en componentes y CRUDs para los catálogos, RBAC y business info.

## Flujo de autenticación

- Inicio de sesión: `POST /auth/login` entrega access token (memoria) y refresh token (cookie HttpOnly, Secure, SameSite=None, Path=/auth/refresh).
- Interceptor Axios: si el access token expira, intenta `/auth/refresh` y repite la llamada original sin interacción del usuario.
- Logout: `POST /auth/logout` revoca el refresh y la sesión, limpiando el estado en el cliente.
- Las políticas de autorización se basan en módulos y acciones, y el cliente oculta/bloquea opciones según los permisos obtenidos en `/auth/me`.

## Scripts útiles

- Ejecutar pruebas de integración (si existiesen): `dotnet test projectSales.sln`
- Linter frontend: `npm run lint`
- Build frontend: `npm run build`

## Usuarios seed

- `master` / `admin` (rol Master)
- `ventas` / `@@ventas2025` (rol Ventas)

Ambos pueden usarse para validar la aplicación end-to-end.
