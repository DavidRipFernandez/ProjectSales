import { NavLink } from 'react-router-dom';
import { useMemo } from 'react';
import { useAuth } from '../app/auth/AuthContext';
import {
  Home,
  Users,
  ShieldCheck,
  Settings,
  PackageSearch,
  Layers,
  Boxes,
  Store,
  Contact2,
  Link2,
  DollarSign,
  Building,
  History
} from 'lucide-react';

const navItems = [
  { label: 'Inicio', path: '/', icon: Home, module: null },
  { label: 'Usuarios', path: '/rbac/users', icon: Users, module: 'Users' },
  { label: 'Roles', path: '/rbac/roles', icon: ShieldCheck, module: 'Roles' },
  { label: 'Permisos', path: '/rbac/permissions', icon: Settings, module: 'Permissions' },
  { label: 'Módulos', path: '/rbac/modules', icon: PackageSearch, module: 'Permissions' },
  { label: 'Marcas', path: '/catalogs/marcas', icon: Layers, module: 'Marcas' },
  { label: 'Categorías', path: '/catalogs/categorias', icon: Boxes, module: 'Materiales' },
  { label: 'Materiales', path: '/catalogs/materiales', icon: PackageSearch, module: 'Materiales' },
  { label: 'Proveedores', path: '/catalogs/proveedores', icon: Store, module: 'Proveedores' },
  { label: 'Contactos', path: '/catalogs/contactos', icon: Contact2, module: 'Proveedores' },
  { label: 'Proveedor-Marcas', path: '/catalogs/proveedores-marcas', icon: Link2, module: 'Proveedores' },
  { label: 'Precios', path: '/catalogs/precios', icon: DollarSign, module: 'Precios' },
  { label: 'Empresa', path: '/business-info', icon: Building, module: 'BusinessInfo' },
  { label: 'Sesiones', path: '/sessions', icon: History, module: null }
];

export const Sidebar = ({ open }: { open: boolean }) => {
  const { hasPermission } = useAuth();

  const items = useMemo(() => navItems.filter((item) => {
    if (!item.module) return true;
    return hasPermission(item.module, 'Read');
  }), [hasPermission]);

  return (
    <aside className={`flex h-full w-64 flex-col border-r border-slate-200 bg-white transition-transform ${open ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}`}>
      <nav className="flex-1 space-y-1 p-4">
        {items.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.path}
              to={item.path}
              end={item.path === '/'}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors ${isActive ? 'bg-brand text-white' : 'text-slate-600 hover:bg-slate-100'}`
              }
            >
              <Icon className="h-4 w-4" />
              {item.label}
            </NavLink>
          );
        })}
      </nav>
    </aside>
  );
};
