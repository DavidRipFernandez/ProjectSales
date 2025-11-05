import { createBrowserRouter } from 'react-router-dom';
import { ProtectedLayout } from './layout/ProtectedLayout';
import { LoginPage } from '../modules/auth/LoginPage';
import { Home } from '../modules/dashboard/Home';
import { UsersPage } from '../modules/rbac/Users';
import { RolesPage } from '../modules/rbac/Roles';
import { PermissionsPage } from '../modules/rbac/Permissions';
import { ModulesPage } from '../modules/rbac/Modules';
import { MarcasPage } from '../modules/marcas/MarcasPage';
import { CategoriasPage } from '../modules/categorias/CategoriasPage';
import { MaterialesPage } from '../modules/materiales/MaterialesPage';
import { ProveedoresPage } from '../modules/proveedores/ProveedoresPage';
import { ContactosPage } from '../modules/contactos/ContactosPage';
import { ProveedorMarcasPage } from '../modules/proveedores-marcas/ProveedorMarcasPage';
import { PreciosPage } from '../modules/precios/PreciosPage';
import { BusinessInfoPage } from '../modules/business-info/BusinessInfoPage';
import { SessionsPage } from '../modules/sessions/SessionsPage';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/',
    element: <ProtectedLayout />,
    children: [
      { index: true, element: <Home /> },
      { path: 'rbac/users', element: <UsersPage /> },
      { path: 'rbac/roles', element: <RolesPage /> },
      { path: 'rbac/permissions', element: <PermissionsPage /> },
      { path: 'rbac/modules', element: <ModulesPage /> },
      { path: 'catalogs/marcas', element: <MarcasPage /> },
      { path: 'catalogs/categorias', element: <CategoriasPage /> },
      { path: 'catalogs/materiales', element: <MaterialesPage /> },
      { path: 'catalogs/proveedores', element: <ProveedoresPage /> },
      { path: 'catalogs/contactos', element: <ContactosPage /> },
      { path: 'catalogs/proveedores-marcas', element: <ProveedorMarcasPage /> },
      { path: 'catalogs/precios', element: <PreciosPage /> },
      { path: 'business-info', element: <BusinessInfoPage /> },
      { path: 'sessions', element: <SessionsPage /> }
    ]
  }
]);
