import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface MarcaDto {
  marcaId: number;
  nombre: string;
  descripcion?: string;
  estado: boolean;
}

const emptyForm = { id: null as number | null, nombre: '', descripcion: '', estado: true };

export const MarcasPage = () => {
  const { hasPermission } = useAuth();
  const [marcas, setMarcas] = useState<MarcaDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Marcas', 'Create');
  const canUpdate = hasPermission('Marcas', 'Update');
  const canDelete = hasPermission('Marcas', 'Delete');

  const loadMarcas = async () => {
    const response = await api.get('/marcas');
    setMarcas(response.data?.data ?? []);
  };

  useEffect(() => {
    loadMarcas().catch(() => setError('No se pudieron cargar las marcas'));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      if (form.id) {
        await api.put(`/marcas/${form.id}`, {
          nombre: form.nombre,
          descripcion: form.descripcion,
          estado: form.estado
        });
      } else {
        await api.post('/marcas', {
          nombre: form.nombre,
          descripcion: form.descripcion,
          estado: form.estado
        });
      }
      setForm(emptyForm);
      await loadMarcas();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar la marca');
    }
  };

  const handleEdit = (marca: MarcaDto) => {
    setForm({ id: marca.marcaId, nombre: marca.nombre, descripcion: marca.descripcion ?? '', estado: marca.estado });
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Deseas deshabilitar esta marca?')) return;
    try {
      await api.delete(`/marcas/${id}`);
      await loadMarcas();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar la marca');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Marcas</h2>
        <p className="text-sm text-slate-500">Gestiona las marcas de productos.</p>
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
              {marcas.map((marca) => (
                <tr key={marca.marcaId} className={!marca.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{marca.nombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{marca.descripcion || '—'}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(marca)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(marca.marcaId)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {marcas.length === 0 && <p className="p-4 text-sm text-slate-500">No hay marcas registradas.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar marca' : 'Nueva marca'}</h3>
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
                label="Activa"
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
