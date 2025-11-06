import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface CategoriaDto {
  categoriaId: number;
  nombre: string;
}

interface MaterialDto {
  materialId: number;
  codigo: string;
  nombre: string;
  descripcion?: string;
  unidadMedida: string;
  sku?: string;
  categoriaId?: number;
  estado: boolean;
  categoriaNombre?: string;
}

const emptyForm = {
  id: null as number | null,
  codigo: '',
  nombre: '',
  descripcion: '',
  unidadMedida: '',
  sku: '',
  categoriaId: '' as number | '' ,
  estado: true
};

export const MaterialesPage = () => {
  const { hasPermission } = useAuth();
  const [materiales, setMateriales] = useState<MaterialDto[]>([]);
  const [categorias, setCategorias] = useState<CategoriaDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [filters, setFilters] = useState({ search: '', categoriaId: '', estado: 'todos' });
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Materiales', 'Create');
  const canUpdate = hasPermission('Materiales', 'Update');
  const canDelete = hasPermission('Materiales', 'Delete');

  const loadCategorias = async () => {
    const response = await api.get('/categorias');
    setCategorias(response.data?.data ?? []);
  };

  const loadMateriales = async () => {
    const params: Record<string, any> = {};
    if (filters.search) params.search = filters.search;
    if (filters.categoriaId) params.categoriaId = filters.categoriaId;
    if (filters.estado !== 'todos') params.estado = filters.estado === 'activos';
    const response = await api.get('/materiales', { params });
    setMateriales(response.data?.data ?? []);
  };

  useEffect(() => {
    Promise.all([loadCategorias(), loadMateriales()]).catch(() => setError('No se pudieron cargar los materiales'));
  }, []);

  useEffect(() => {
    loadMateriales().catch(() => setError('No se pudieron filtrar los materiales'));
  }, [filters.categoriaId, filters.estado]);

  const handleFilterSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    loadMateriales().catch(() => setError('No se pudieron filtrar los materiales'));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      const payload = {
        codigo: form.codigo,
        nombre: form.nombre,
        descripcion: form.descripcion,
        unidadMedida: form.unidadMedida,
        sku: form.sku,
        categoriaId: form.categoriaId === '' ? null : form.categoriaId,
        estado: form.estado
      };
      if (form.id) {
        await api.put(`/materiales/${form.id}`, payload);
      } else {
        await api.post('/materiales', payload);
      }
      setForm(emptyForm);
      await loadMateriales();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el material');
    }
  };

  const handleEdit = (material: MaterialDto) => {
    setForm({
      id: material.materialId,
      codigo: material.codigo,
      nombre: material.nombre,
      descripcion: material.descripcion ?? '',
      unidadMedida: material.unidadMedida,
      sku: material.sku ?? '',
      categoriaId: material.categoriaId ?? '',
      estado: material.estado
    });
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Deseas deshabilitar este material?')) return;
    try {
      await api.delete(`/materiales/${id}`);
      await loadMateriales();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el material');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Materiales</h2>
        <p className="text-sm text-slate-500">Administra el catálogo de materiales.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <form onSubmit={handleFilterSubmit} className="grid gap-4 rounded-xl border border-slate-200 bg-white p-4 shadow-sm md:grid-cols-4">
        <FormField
          label="Buscar"
          name="search"
          value={filters.search}
          onChange={(value) => setFilters((prev) => ({ ...prev, search: value }))}
          placeholder="Código, nombre o SKU"
        />
        <FormField
          label="Categoría"
          name="categoria"
          type="select"
          value={filters.categoriaId}
          onChange={(value) => setFilters((prev) => ({ ...prev, categoriaId: value }))}
          options={categorias.map((categoria) => ({ value: categoria.categoriaId, label: categoria.nombre }))}
        />
        <FormField
          label="Estado"
          name="estado"
          type="select"
          value={filters.estado}
          onChange={(value) => setFilters((prev) => ({ ...prev, estado: value }))}
          options={[{ value: 'activos', label: 'Activos' }, { value: 'inactivos', label: 'Inactivos' }]}
        />
        <div className="flex items-end justify-end">
          <Button type="submit">Filtrar</Button>
        </div>
      </form>
      <div className="grid gap-6 xl:grid-cols-[2fr_1fr]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Código</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Nombre</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Categoría</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {materiales.map((material) => (
                <tr key={material.materialId} className={!material.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{material.codigo}</td>
                  <td className="px-4 py-3 text-sm text-slate-600">
                    <div className="font-semibold text-slate-700">{material.nombre}</div>
                    <div className="text-xs text-slate-500">SKU: {material.sku || '—'} · UM: {material.unidadMedida}</div>
                  </td>
                  <td className="px-4 py-3 text-sm text-slate-500">{material.categoriaNombre || 'Sin categoría'}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(material)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(material.materialId)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {materiales.length === 0 && <p className="p-4 text-sm text-slate-500">No hay materiales registrados.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar material' : 'Nuevo material'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Código"
                name="codigo"
                value={form.codigo}
                onChange={(value) => handleChange('codigo', value)}
                required
              />
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
                label="Unidad de medida"
                name="unidadMedida"
                value={form.unidadMedida}
                onChange={(value) => handleChange('unidadMedida', value)}
                required
              />
              <FormField
                label="SKU"
                name="sku"
                value={form.sku}
                onChange={(value) => handleChange('sku', value)}
              />
              <FormField
                label="Categoría"
                name="categoriaId"
                type="select"
                value={form.categoriaId}
                onChange={(value) => handleChange('categoriaId', value === '' ? '' : Number(value))}
                options={categorias.map((categoria) => ({ value: categoria.categoriaId, label: categoria.nombre }))}
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
