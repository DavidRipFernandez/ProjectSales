import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface ProveedorDto {
  proveedorCifId: string;
  nombre: string;
}

interface ContactoDto {
  contactoId: number;
  nombre: string;
  correo?: string;
  telefono: string;
  descripcion?: string;
  estado: boolean;
}

const emptyForm = { id: null as number | null, nombre: '', correo: '', telefono: '', descripcion: '', estado: true };

export const ContactosPage = () => {
  const { hasPermission } = useAuth();
  const [proveedores, setProveedores] = useState<ProveedorDto[]>([]);
  const [selectedProveedor, setSelectedProveedor] = useState<string>('');
  const [contactos, setContactos] = useState<ContactoDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission('Proveedores', 'Create');
  const canUpdate = hasPermission('Proveedores', 'Update');
  const canDelete = hasPermission('Proveedores', 'Delete');

  const loadProveedores = async () => {
    const response = await api.get('/proveedores');
    setProveedores(response.data?.data ?? []);
    if (!selectedProveedor && response.data?.data?.length) {
      setSelectedProveedor(response.data.data[0].proveedorCifId);
    }
  };

  const loadContactos = async (proveedorId: string) => {
    if (!proveedorId) {
      setContactos([]);
      return;
    }
    const response = await api.get(`/proveedores/${proveedorId}/contactos`);
    setContactos(response.data?.data ?? []);
  };

  useEffect(() => {
    loadProveedores().catch(() => setError('No se pudieron cargar los proveedores'));
  }, []);

  useEffect(() => {
    if (selectedProveedor) {
      loadContactos(selectedProveedor).catch(() => setError('No se pudieron cargar los contactos'));
    }
  }, [selectedProveedor]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!selectedProveedor) return;
    setError(null);
    try {
      const payload = {
        nombre: form.nombre,
        correo: form.correo,
        telefono: form.telefono,
        descripcion: form.descripcion,
        estado: form.estado
      };
      if (form.id) {
        await api.put(`/proveedores/${selectedProveedor}/contactos/${form.id}`, payload);
      } else {
        await api.post(`/proveedores/${selectedProveedor}/contactos`, payload);
      }
      setForm(emptyForm);
      await loadContactos(selectedProveedor);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo guardar el contacto');
    }
  };

  const handleEdit = (contacto: ContactoDto) => {
    setForm({
      id: contacto.contactoId,
      nombre: contacto.nombre,
      correo: contacto.correo ?? '',
      telefono: contacto.telefono,
      descripcion: contacto.descripcion ?? '',
      estado: contacto.estado
    });
  };

  const handleDelete = async (contactoId: number) => {
    if (!selectedProveedor) return;
    if (!confirm('¿Deseas deshabilitar este contacto?')) return;
    try {
      await api.delete(`/proveedores/${selectedProveedor}/contactos/${contactoId}`);
      await loadContactos(selectedProveedor);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo deshabilitar el contacto');
    }
  };

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <h2 className="text-2xl font-semibold text-slate-800">Contactos</h2>
          <p className="text-sm text-slate-500">Administra los contactos asociados a cada proveedor.</p>
        </div>
        <div className="flex items-center gap-3">
          <label className="text-sm font-medium text-slate-700">
            Proveedor
            <select
              className="ml-3 rounded-md border border-slate-200 px-3 py-2 text-sm focus:border-brand focus:outline-none"
              value={selectedProveedor}
              onChange={(event) => setSelectedProveedor(event.target.value)}
            >
              {proveedores.map((proveedor) => (
                <option key={proveedor.proveedorCifId} value={proveedor.proveedorCifId}>
                  {proveedor.nombre}
                </option>
              ))}
            </select>
          </label>
        </div>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Nombre</th>
                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">Contacto</th>
                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-500">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {contactos.map((contacto) => (
                <tr key={contacto.contactoId} className={!contacto.estado ? 'bg-slate-100' : ''}>
                  <td className="px-4 py-3 text-sm font-medium text-slate-700">{contacto.nombre}</td>
                  <td className="px-4 py-3 text-sm text-slate-500">
                    <div>{contacto.correo || 'Sin correo'}</div>
                    <div className="text-xs text-slate-400">{contacto.telefono}</div>
                    <div className="text-xs text-slate-400">{contacto.descripcion || '—'}</div>
                  </td>
                  <td className="px-4 py-3 text-right text-sm">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <Button variant="secondary" onClick={() => handleEdit(contacto)}>
                          Editar
                        </Button>
                      )}
                      {canDelete && (
                        <Button variant="ghost" onClick={() => handleDelete(contacto.contactoId)}>
                          Deshabilitar
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {contactos.length === 0 && <p className="p-4 text-sm text-slate-500">No hay contactos registrados.</p>}
        </div>
        {(canCreate || (canUpdate && form.id)) && (
          <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{form.id ? 'Editar contacto' : 'Nuevo contacto'}</h3>
            <form onSubmit={handleSubmit} className="mt-4 space-y-4">
              <FormField
                label="Nombre"
                name="nombre"
                value={form.nombre}
                onChange={(value) => handleChange('nombre', value)}
                required
              />
              <FormField
                label="Correo"
                name="correo"
                type="email"
                value={form.correo}
                onChange={(value) => handleChange('correo', value)}
              />
              <FormField
                label="Teléfono"
                name="telefono"
                value={form.telefono}
                onChange={(value) => handleChange('telefono', value)}
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
