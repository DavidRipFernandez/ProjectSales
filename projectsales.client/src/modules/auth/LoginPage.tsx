import { useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../app/auth/AuthContext';
import { Button } from '../../components/Button';
import { FormField } from '../../components/FormField';

export const LoginPage = () => {
  const { login, user, loading } = useAuth();
  const location = useLocation();
  const [form, setForm] = useState({ usernameOrEmail: '', password: '' });
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleChange = (field: 'usernameOrEmail' | 'password', value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      await login(form.usernameOrEmail, form.password);
    } catch (err: any) {
      setError(err?.message ?? 'No se pudo iniciar sesión');
    } finally {
      setSubmitting(false);
    }
  };

  if (!loading && user) {
    const redirectTo = (location.state as any)?.from?.pathname ?? '/';
    return <Navigate to={redirectTo} replace />;
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 p-6">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-xl">
        <h1 className="text-2xl font-bold text-slate-800">Bienvenido</h1>
        <p className="mt-2 text-sm text-slate-500">Ingresa tus credenciales para acceder al sistema.</p>
        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <FormField
            label="Usuario o correo"
            name="usernameOrEmail"
            value={form.usernameOrEmail}
            onChange={(value) => handleChange('usernameOrEmail', value)}
            placeholder="master"
            required
          />
          <FormField
            label="Contraseña"
            name="password"
            type="password"
            value={form.password}
            onChange={(value) => handleChange('password', value)}
            placeholder="•••••••"
            required
          />
          {error && <p className="text-sm text-red-500">{error}</p>}
          <Button type="submit" className="w-full" disabled={submitting}>
            {submitting ? 'Ingresando…' : 'Ingresar'}
          </Button>
        </form>
      </div>
    </div>
  );
};
