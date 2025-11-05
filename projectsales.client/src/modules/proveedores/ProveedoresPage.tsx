import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface ProveedorDto {
  proveedorCifId: string;
  nombre: string;
  domicilioSocial: string;
  estado: boolean;
}

const emptyForm = { id: '', nombre: '', domicilio: '', estado: true };

export const ProveedoresPage = () => {
  const { hasPermission } = useAuth();
  const [proveedores, setProveedores] = useState<ProveedorDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Proveedores', 'Create');
  const canUpdate = hasPermission('Proveedores', 'Update');
  const canDelete = hasPermission('Proveedores', 'Delete');

  const loadProveedores = async () => {
    const response = await api.get('/proveedores');
    setProveedores(response.data?.data ?? []);
  };

  useEffect(() => {
    loadProveedores().catch(() => setError('No se pudieron cargar los proveedores'));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      if (form.id && proveedores.some((p) => p.proveedorCifId === form.id)) {
        await api.put(`/proveedores/${form.id}`, {
          proveedorCifId: form.id,
          nombre: form.nombre,
          domicilioSocial: form.domicilio,
          estado: form.estado
        });
      } else {
        await api.post('/proveedores', {
          proveedorCifId: form.id,
          nombre: form.nombre,
          domicilioSocial: form.domicilio,
          estado: form.estado
        });
      }
      setForm(emptyForm);
      await loadProveedores();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el proveedor');
    }
  };

  const handleEdit = (proveedor: ProveedorDto) => {
    setForm({ id: proveedor.proveedorCifId, nombre: proveedor.nombre, domicilio: proveedor.domicilioSocial, estado: proveedor.estado });
  };

  const handleDelete = async (id: string) => {
    if (!confirm('¿Deseas deshabilitar este proveedor?')) return;
    try {
      await api.delete(`/proveedores/${id}`);
      await loadProveedores();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el proveedor');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Proveedores</h2>
        <p className="text-sm text-slate-500">Gestiona la información de proveedores.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">NIT/CIF</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Nombre</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Domicilio</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {proveedores.map((proveedor) => (
                <tr key={proveedor.proveedorCifId} className={!proveedor.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{proveedor.proveedorCifId}</td>
                  <td className="px-4 py-3 text-sm text-slate-600">{proveedor.nombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{proveedor.domicilioSocial}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(proveedor)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(proveedor.proveedorCifId)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {proveedores.length === 0 && <p className="p-4 text-sm text-slate-500">No hay proveedores registrados.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{proveedores.some((p) => p.proveedorCifId === form.id) ? 'Editar proveedor' : 'Nuevo proveedor'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="NIT/CIF"
                name="id"
                value={form.id}
                onChange={(value) => handleChange('id', value)}
                required
                disabled={proveedores.some((p) => p.proveedorCifId === form.id)}
              />
              <FormField
                label="Nombre"
                name="nombre"
                value={form.nombre}
                onChange={(value) => handleChange('nombre', value)}
                required
              />
              <FormField
                label="Domicilio"
                name="domicilio"
                value={form.domicilio}
                onChange={(value) => handleChange('domicilio', value)}
                required
              />
              <FormField
                label="Activo"
                name="estado"
                type="checkbox"
                value={form.estado}
                onChange={(value) => handleChange('estado', value)}
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
