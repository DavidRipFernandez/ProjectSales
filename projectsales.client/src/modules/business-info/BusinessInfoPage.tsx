import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { FormField } from '../../components/FormField';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface BusinessInfoDto {
  id: number;
  empresaNombre: string;
  nit?: string;
  contactoNombre?: string;
  contactoTelefono?: string;
  contactoEmail?: string;
  direccion?: string;
  ciudad?: string;
  pais?: string;
  web?: string;
  facebook?: string;
  instagram?: string;
  tiktok?: string;
  isPrimary: boolean;
}

const emptyForm = {
  empresaNombre: '',
  nit: '',
  contactoNombre: '',
  contactoTelefono: '',
  contactoEmail: '',
  direccion: '',
  ciudad: '',
  pais: '',
  web: '',
  facebook: '',
  instagram: '',
  tiktok: ''
};

export const BusinessInfoPage = () => {
  const { hasPermission } = useAuth();
  const [form, setForm] = useState(emptyForm);
  const [status, setStatus] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const canUpdate = hasPermission('BusinessInfo', 'Update');

  const loadInfo = async () => {
    const response = await api.get('/business-info');
    const info: BusinessInfoDto | null = response.data?.data ?? null;
    if (info) {
      setForm({
        empresaNombre: info.empresaNombre,
        nit: info.nit ?? '',
        contactoNombre: info.contactoNombre ?? '',
        contactoTelefono: info.contactoTelefono ?? '',
        contactoEmail: info.contactoEmail ?? '',
        direccion: info.direccion ?? '',
        ciudad: info.ciudad ?? '',
        pais: info.pais ?? '',
        web: info.web ?? '',
        facebook: info.facebook ?? '',
        instagram: info.instagram ?? '',
        tiktok: info.tiktok ?? ''
      });
    }
  };

  useEffect(() => {
    loadInfo().catch(() => setError('No se pudo cargar la información de la empresa'));
  }, []);

  const handleChange = (key: string, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!canUpdate) return;
    setError(null);
    setStatus(null);
    try {
      await api.put('/business-info', form);
      setStatus('Información actualizada correctamente.');
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo actualizar la información');
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Información de la empresa</h2>
        <p className="text-sm text-slate-500">Actualiza los datos de contacto y presencia digital.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      {status && <div className="rounded-md border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-600">{status}</div>}
      <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <form onSubmit={handleSubmit} className="grid gap-4 md:grid-cols-2">
          <FormField
            label="Nombre de la empresa"
            name="empresaNombre"
            value={form.empresaNombre}
            onChange={(value) => handleChange('empresaNombre', value)}
            required
          />
          <FormField
            label="NIT"
            name="nit"
            value={form.nit}
            onChange={(value) => handleChange('nit', value)}
          />
          <FormField
            label="Contacto"
            name="contactoNombre"
            value={form.contactoNombre}
            onChange={(value) => handleChange('contactoNombre', value)}
          />
          <FormField
            label="Teléfono"
            name="contactoTelefono"
            value={form.contactoTelefono}
            onChange={(value) => handleChange('contactoTelefono', value)}
          />
          <FormField
            label="Correo"
            name="contactoEmail"
            type="email"
            value={form.contactoEmail}
            onChange={(value) => handleChange('contactoEmail', value)}
          />
          <FormField
            label="Dirección"
            name="direccion"
            value={form.direccion}
            onChange={(value) => handleChange('direccion', value)}
          />
          <FormField
            label="Ciudad"
            name="ciudad"
            value={form.ciudad}
            onChange={(value) => handleChange('ciudad', value)}
          />
          <FormField
            label="País"
            name="pais"
            value={form.pais}
            onChange={(value) => handleChange('pais', value)}
          />
          <FormField
            label="Sitio web"
            name="web"
            value={form.web}
            onChange={(value) => handleChange('web', value)}
          />
          <FormField
            label="Facebook"
            name="facebook"
            value={form.facebook}
            onChange={(value) => handleChange('facebook', value)}
          />
          <FormField
            label="Instagram"
            name="instagram"
            value={form.instagram}
            onChange={(value) => handleChange('instagram', value)}
          />
          <FormField
            label="TikTok"
            name="tiktok"
            value={form.tiktok}
            onChange={(value) => handleChange('tiktok', value)}
          />
          <div className="md:col-span-2 flex justify-end">
            <Button type="submit" disabled={!canUpdate}>Guardar cambios</Button>
          </div>
        </form>
      </div>
    </div>
  );
};
