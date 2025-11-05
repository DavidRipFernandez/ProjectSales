import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { Button } from '../../components/Button';
import { FormField } from '../../components/FormField';
import { useAuth } from '../../app/auth/AuthContext';
import { Can } from '../../app/auth/Permission';

interface UserItem {
  id: number;
  username: string;
  email: string;
  activo: boolean;
  roles: string[];
}

interface RoleItem {
  id: number;
  nombre: string;
}

const emptyForm = {
  id: null as number | null,
  username: '',
  email: '',
  password: '',
  activo: true,
  roleIds: [] as number[]
};

export const UsersPage = () => {
  const { hasPermission } = useAuth();
  const [users, setUsers] = useState<UserItem[]>([]);
  const [roles, setRoles] = useState<RoleItem[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Users', 'Create');
  const canUpdate = hasPermission('Users', 'Update');
  const canDelete = hasPermission('Users', 'Delete');

  const loadData = async () => {
    setLoading(true);
    try {
      const [usersResponse, rolesResponse] = await Promise.all([
        api.get('/users'),
        api.get('/roles')
      ]);
      setUsers(usersResponse.data?.data ?? []);
      setRoles(rolesResponse.data?.data ?? []);
    } catch (err: any) {
      setError(err?.message ?? 'No se pudieron cargar los usuarios');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleRoleToggle = (roleId: number) => {
    setForm((prev) => {
      const roleIds = prev.roleIds.includes(roleId)
        ? prev.roleIds.filter((id) => id !== roleId)
        : [...prev.roleIds, roleId];
      return { ...prev, roleIds };
    });
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      if (form.id) {
        await api.put(`/users/${form.id}`, {
          username: form.username,
          email: form.email,
          activo: form.activo,
          password: form.password || null,
          roleIds: form.roleIds
        });
      } else {
        await api.post('/users', {
          username: form.username,
          email: form.email,
          password: form.password,
          activo: form.activo,
          roleIds: form.roleIds
        });
      }
      setForm(emptyForm);
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el usuario');
    }
  };

  const handleEdit = (user: UserItem) => {
    setForm({
      id: user.id,
      username: user.username,
      email: user.email,
      password: '',
      activo: user.activo,
      roleIds: roles
        .filter((role) => user.roles.includes(role.nombre))
        .map((role) => role.id)
    });
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Deseas deshabilitar este usuario?')) return;
    try {
      await api.delete(`/users/${id}`);
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el usuario');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold text-slate-800">Usuarios</h2>
          <p className="text-sm text-slate-500">Gestiona los usuarios y sus roles de acceso.</p>
        </div>
      </div>

      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}

      <div className="grid gap-6 lg:grid-cols-[1fr_360px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Usuario</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Correo</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Roles</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {users.map((user) => (
                <tr key={user.id} className={!user.activo ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{user.username}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{user.email}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{user.roles.join(', ') || 'Sin roles'}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      <Can module="Users" action="Update">
                        <Button variant="secondary" onClick={() => handleEdit(user)}>
                          Editar
                        </Button>
                      </Can>
                      <Can module="Users" action="Delete">
                        <Button variant="ghost" onClick={() => handleDelete(user.id)}>
                          Deshabilitar
                        </Button>
                      </Can>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {loading && <p className="p-4 text-sm text-slate-500">Cargando usuarios…</p>}
          {!loading && users.length === 0 && <p className="p-4 text-sm text-slate-500">No hay usuarios registrados.</p>}
        </div>

        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar usuario' : 'Nuevo usuario'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Usuario"
                name="username"
                value={form.username}
                onChange={(value) => handleChange('username', value)}
                required
              />
              <FormField
                label="Correo"
                name="email"
                type="email"
                value={form.email}
                onChange={(value) => handleChange('email', value)}
                required
              />
              <FormField
                label="Contraseña"
                name="password"
                type="password"
                value={form.password}
                onChange={(value) => handleChange('password', value)}
                placeholder={form.id ? 'Dejar en blanco para mantener' : ''}
                required={!form.id}
              />
              <FormField
                label="Activo"
                name="activo"
                type="checkbox"
                value={form.activo}
                onChange={(value) => handleChange('activo', value)}
              />
              <div className="space-y-2">
                <p className="text-sm font-medium text-slate-700">Roles</p>
                <div className="flex flex-wrap gap-2">
                  {roles.map((role) => (
                    <label key={role.id} className="flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm">
                      <input
                        type="checkbox"
                        checked={form.roleIds.includes(role.id)}
                        onChange={() => handleRoleToggle(role.id)}
                      />
                      {role.nombre}
                    </label>
                  ))}
                </div>
              </div>
              <div className="flex justify-between">
                <Button type="submit" disabled={!form.id && !canCreate}>
                  Guardar
                </Button>
                {form.id && (
                  <Button type="button" variant="ghost" onClick={() => setForm(emptyForm)}>
                    Cancelar
                  </Button>
                )}
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
};
