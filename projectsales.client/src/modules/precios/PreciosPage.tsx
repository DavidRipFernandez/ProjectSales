import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface MaterialDto {
  materialId: number;
  codigo: string;
  nombre: string;
}

interface ProveedorDto {
  proveedorCifId: string;
  nombre: string;
}

interface MarcaDto {
  marcaId: number;
  nombre: string;
}

interface PrecioDto {
  materialId: number;
  materialCodigo: string;
  materialNombre: string;
  proveedorCifId: string;
  proveedorNombre: string;
  marcaId: number;
  marcaNombre: string;
  precio: number;
  estado: boolean;
}

const emptyForm = { materialId: '' as number | '', proveedorCifId: '', marcaId: '' as number | '', precio: '', estado: true, editing: false };

export const PreciosPage = () => {
  const { hasPermission } = useAuth();
  const [materiales, setMateriales] = useState<MaterialDto[]>([]);
  const [proveedores, setProveedores] = useState<ProveedorDto[]>([]);
  const [marcas, setMarcas] = useState<MarcaDto[]>([]);
  const [precios, setPrecios] = useState<PrecioDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Precios', 'Create');
  const canUpdate = hasPermission('Precios', 'Update');
  const canDelete = hasPermission('Precios', 'Delete');

  const loadData = async () => {
    const [materialesResponse, proveedoresResponse, marcasResponse, preciosResponse] = await Promise.all([
      api.get('/materiales'),
      api.get('/proveedores'),
      api.get('/marcas'),
      api.get('/precios')
    ]);
    setMateriales(materialesResponse.data?.data ?? []);
    setProveedores(proveedoresResponse.data?.data ?? []);
    setMarcas(marcasResponse.data?.data ?? []);
    setPrecios(preciosResponse.data?.data ?? []);
  };

  useEffect(() => {
    loadData().catch(() => setError('No se pudieron cargar los precios'));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (form.materialId === '' || !form.proveedorCifId || form.marcaId === '') return;
    setError(null);
    try {
      const payload = {
        materialId: form.materialId,
        proveedorCifId: form.proveedorCifId,
        marcaId: form.marcaId,
        precio: Number(form.precio),
        estado: form.estado
      };
      if (form.editing) {
        await api.put('/precios', payload);
      } else {
        await api.post('/precios', payload);
      }
      setForm(emptyForm);
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el precio');
    }
  };

  const handleEdit = (precio: PrecioDto) => {
    setForm({
      materialId: precio.materialId,
      proveedorCifId: precio.proveedorCifId,
      marcaId: precio.marcaId,
      precio: precio.precio.toString(),
      estado: precio.estado,
      editing: true
    });
  };

  const handleDelete = async (precio: PrecioDto) => {
    if (!confirm('¿Deseas deshabilitar este precio?')) return;
    try {
      await api.delete('/precios', { params: { materialId: precio.materialId, proveedorId: precio.proveedorCifId, marcaId: precio.marcaId } });
      await loadData();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el precio');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Precios</h2>
        <p className="text-sm text-slate-500">Administra los precios por material, proveedor y marca.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-6 xl:grid-cols-[1fr_360px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Material</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Proveedor</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Marca</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Precio</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {precios.map((precio) => (
                <tr key={`${precio.materialId}-${precio.proveedorCifId}-${precio.marcaId}`} className={!precio.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{precio.materialCodigo} · {precio.materialNombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{precio.proveedorNombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{precio.marcaNombre}</td>
                  <td className="px-4 py-3 text-right text-sm text-slate-700">Bs {precio.precio.toFixed(2)}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(precio)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(precio)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {precios.length === 0 && <p className="p-4 text-sm text-slate-500">No hay precios registrados.</p>}
        </div>
        {(canCreate || (canUpdate && form.editing)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.editing ? 'Editar precio' : 'Nuevo precio'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Material"
                name="materialId"
                type="select"
                value={form.materialId}
                onChange={(value) => handleChange('materialId', value === '' ? '' : Number(value))}
                options={materiales.map((material) => ({ value: material.materialId, label: `${material.codigo} · ${material.nombre}` }))}
                required
              />
              <FormField
                label="Proveedor"
                name="proveedorCifId"
                type="select"
                value={form.proveedorCifId}
                onChange={(value) => handleChange('proveedorCifId', value)}
                options={proveedores.map((proveedor) => ({ value: proveedor.proveedorCifId, label: proveedor.nombre }))}
                required
              />
              <FormField
                label="Marca"
                name="marcaId"
                type="select"
                value={form.marcaId}
                onChange={(value) => handleChange('marcaId', value === '' ? '' : Number(value))}
                options={marcas.map((marca) => ({ value: marca.marcaId, label: marca.nombre }))}
                required
              />
              <FormField
                label="Precio"
                name="precio"
                type="number"
                value={form.precio}
                onChange={(value) => handleChange('precio', value)}
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
                <Button type="submit" disabled={!form.editing && !canCreate}>Guardar</Button>
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
