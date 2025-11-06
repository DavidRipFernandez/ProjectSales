import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface ProveedorDto {
  proveedorCifId: string;
  nombre: string;
}

interface MarcaDto {
  marcaId: number;
  nombre: string;
}

interface ProveedorMarcaDto {
  proveedorCifId: string;
  marcaId: number;
  estado: boolean;
  marcaNombre: string;
}

const emptyForm = { proveedorCifId: '', marcaId: '' as number | '', estado: true, editing: false };

export const ProveedorMarcasPage = () => {
  const { hasPermission } = useAuth();
  const [proveedores, setProveedores] = useState<ProveedorDto[]>([]);
  const [marcas, setMarcas] = useState<MarcaDto[]>([]);
  const [relations, setRelations] = useState<ProveedorMarcaDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Proveedores', 'Create');
  const canUpdate = hasPermission('Proveedores', 'Update');
  const canDelete = hasPermission('Proveedores', 'Delete');

  const loadData = async () => {
    const [proveedoresResponse, marcasResponse, relationsResponse] = await Promise.all([
      api.get('/proveedores'),
      api.get('/marcas'),
      api.get('/proveedores-marcas')
    ]);
    setProveedores(proveedoresResponse.data?.data ?? []);
    setMarcas(marcasResponse.data?.data ?? []);
    setRelations(relationsResponse.data?.data ?? []);
  };

  useEffect(() => {
    loadData().catch(() => setError('No se pudieron cargar las relaciones proveedor-marca'));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!form.proveedorCifId || form.marcaId === '') return;
    setError(null);
    try {
      const payload = {
        proveedorCifId: form.proveedorCifId,
        marcaId: form.marcaId,
        estado: form.estado
      };
      if (form.editing) {
        await api.put('/proveedores-marcas', payload);
      } else {
        await api.post('/proveedores-marcas', payload);
      }
      setForm(emptyForm);
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar la relación');
    }
  };

  const handleEdit = (relation: ProveedorMarcaDto) => {
    setForm({ proveedorCifId: relation.proveedorCifId, marcaId: relation.marcaId, estado: relation.estado, editing: true });
  };

  const handleDelete = async (relation: ProveedorMarcaDto) => {
    if (!confirm('¿Deseas deshabilitar esta relación?')) return;
    try {
      await api.delete(`/proveedores-marcas`, { params: { proveedorCifId: relation.proveedorCifId, marcaId: relation.marcaId } });
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar la relación');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Proveedores por marca</h2>
        <p className="text-sm text-slate-500">Asigna qué marcas distribuye cada proveedor.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Proveedor</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Marca</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {relations.map((relation) => (
                <tr key={`${relation.proveedorCifId}-${relation.marcaId}`} className={!relation.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{relation.proveedorCifId}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{relation.marcaNombre}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(relation)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(relation)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {relations.length === 0 && <p className="p-4 text-sm text-slate-500">No hay relaciones registradas.</p>}
        </div>
        {(canCreate || (canUpdate && form.editing)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.editing ? 'Editar relación' : 'Nueva relación'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Proveedor"
                name="proveedor"
                type="select"
                value={form.proveedorCifId}
                onChange={(value) => handleChange('proveedorCifId', value)}
                options={proveedores.map((proveedor) => ({ value: proveedor.proveedorCifId, label: proveedor.nombre }))}
                required
              />
              <FormField
                label="Marca"
                name="marca"
                type="select"
                value={form.marcaId}
                onChange={(value) => handleChange('marcaId', value === '' ? '' : Number(value))}
                options={marcas.map((marca) => ({ value: marca.marcaId, label: marca.nombre }))}
                required
              />
              <FormField
                label="Activa"
                name="estado"
                type="checkbox"
                value={form.estado}
                onChange={(value) => handleChange('estado', value)}
              />
              <div className="flex justify-between">
                <Button type="submit" disabled={!form.proveedorCifId || form.marcaId === '' || (!form.editing && !canCreate)}>
                  Guardar
                </Button>
                {form.editing && (
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
