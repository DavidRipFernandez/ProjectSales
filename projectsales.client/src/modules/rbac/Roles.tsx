import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { Button } from '../../components/Button';
import { FormField } from '../../components/FormField';
import { useAuth } from '../../app/auth/AuthContext';

interface RoleItem {
  id: number;
  nombre: string;
  descripcion?: string;
  activo: boolean;
}

const emptyForm = { id: null as number | null, nombre: '', descripcion: '', activo: true };

export const RolesPage = () => {
  const { hasPermission } = useAuth();
  const [roles, setRoles] = useState<RoleItem[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Roles', 'Create');
  const canUpdate = hasPermission('Roles', 'Update');
  const canDelete = hasPermission('Roles', 'Delete');

  const loadRoles = async () => {
    try {
      const response = await api.get('/roles');
      setRoles(response.data?.data ?? []);
    } catch (err: any) {
      setError(err?.message ?? 'No se pudieron cargar los roles');
    }
  };

  useEffect(() => {
    loadRoles();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      if (form.id) {
        await api.put(`/roles/${form.id}`, {
          nombre: form.nombre,
          descripcion: form.descripcion,
          activo: form.activo
        });
      } else {
        await api.post('/roles', {
          nombre: form.nombre,
          descripcion: form.descripcion,
          activo: form.activo
        });
      }
      setForm(emptyForm);
      await loadRoles();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el rol');
    }
  };

  const handleEdit = (role: RoleItem) => {
    setForm({ id: role.id, nombre: role.nombre, descripcion: role.descripcion ?? '', activo: role.activo });
  };

  const handleDelete = async (roleId: number) => {
    if (!confirm('¿Deseas deshabilitar este rol?')) return;
    try {
      await api.delete(`/roles/${roleId}`);
      await loadRoles();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el rol');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Roles</h2>
        <p className="text-sm text-slate-500">Controla los roles disponibles en el sistema.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Nombre</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Descripción</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {roles.map((role) => (
                <tr key={role.id} className={!role.activo ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{role.nombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{role.descripcion || '—'}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(role)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(role.id)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {roles.length === 0 && <p className="p-4 text-sm text-slate-500">No hay roles registrados.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar rol' : 'Nuevo rol'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Nombre"
                name="nombre"
                value={form.nombre}
                onChange={(value) => handleChange('nombre', value)}
                required
              />
              <FormField
                label="Descripción"
                name="descripcion"
                type="textarea"
                value={form.descripcion}
                onChange={(value) => handleChange('descripcion', value)}
              />
              <FormField
                label="Activo"
                name="activo"
                type="checkbox"
                value={form.activo}
                onChange={(value) => handleChange('activo', value)}
              />
              <div className="flex justify-between">
                <Button type="submit" disabled={!form.id && !canCreate}>Guardar</Button>
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
