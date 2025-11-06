# projectsales.client

Aplicación React + Vite para la gestión de ProjectSales. Incluye TailwindCSS, lucide-react y React Router con un contexto de autenticación basado en JWT.

## Scripts

- `npm run dev` – arranca el servidor de desarrollo en `http://localhost:5173`.
- `npm run build` – genera el build de producción.
- `npm run lint` – ejecuta ESLint sobre el código fuente.

## Iconos

Se utilizan los iconos de [lucide-react](https://lucide.dev/icons). Para agregar nuevos iconos:

```tsx
import { Plus } from 'lucide-react';

<Plus className="h-4 w-4" />
```

## Notas de autenticación

- Axios está configurado con `withCredentials` para recibir la cookie de refresh.
- El access token se mantiene en memoria dentro de `AuthContext` y se añade automáticamente a cada solicitud.
- Ante un `401`, el interceptor llama a `/auth/refresh` y reintenta la petición original.

## Permisos en UI

Usa el componente `<Can module="Materiales" action="Create">...</Can>` para mostrar/ocultar acciones según los permisos del usuario autenticado.
