import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface CategoriaDto {
  categoriaId: number;
  nombre: string;
  descripcion?: string;
  estado: boolean;
}

const emptyForm = { id: null as number | null, nombre: '', descripcion: '', estado: true };

export const CategoriasPage = () => {
  const { hasPermission } = useAuth();
  const [categorias, setCategorias] = useState<CategoriaDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Materiales', 'Create');
  const canUpdate = hasPermission('Materiales', 'Update');
  const canDelete = hasPermission('Materiales', 'Delete');

  const loadCategorias = async () => {
    const response = await api.get('/categorias');
    setCategorias(response.data?.data ?? []);
  };

  useEffect(() => {
    loadCategorias().catch(() => setError('No se pudieron cargar las categorías'));
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      if (form.id) {
        await api.put(`/categorias/${form.id}`, {
          nombre: form.nombre,
          descripcion: form.descripcion,
          estado: form.estado
        });
      } else {
        await api.post('/categorias', {
          nombre: form.nombre,
          descripcion: form.descripcion,
          estado: form.estado
        });
      }
      setForm(emptyForm);
      await loadCategorias();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar la categoría');
    }
  };

  const handleEdit = (categoria: CategoriaDto) => {
    setForm({ id: categoria.categoriaId, nombre: categoria.nombre, descripcion: categoria.descripcion ?? '', estado: categoria.estado });
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Deseas deshabilitar esta categoría?')) return;
    try {
      await api.delete(`/categorias/${id}`);
      await loadCategorias();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar la categoría');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Categorías</h2>
        <p className="text-sm text-slate-500">Clasifica los materiales por categoría.</p>
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
              {categorias.map((categoria) => (
                <tr key={categoria.categoriaId} className={!categoria.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{categoria.nombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">{categoria.descripcion || '—'}</td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(categoria)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(categoria.categoriaId)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {categorias.length === 0 && <p className="p-4 text-sm text-slate-500">No hay categorías registradas.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar categoría' : 'Nueva categoría'}</h3>
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
