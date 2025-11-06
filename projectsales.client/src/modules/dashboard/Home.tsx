import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { useAuth } from '../../app/auth/AuthContext';
import { EmptyState } from '../../components/EmptyState';

export const Home = () => {
  const { user } = useAuth();
  const [summary, setSummary] = useState<{ materiales: number; proveedores: number; precios: number }>({ materiales: 0, proveedores: 0, precios: 0 });

  useEffect(() => {
    const loadSummary = async () => {
      try {
        const [materiales, proveedores, precios] = await Promise.all([
          api.get('/materiales'),
          api.get('/proveedores'),
          api.get('/precios')
        ]);
        setSummary({
          materiales: materiales.data?.data?.length ?? 0,
          proveedores: proveedores.data?.data?.length ?? 0,
          precios: precios.data?.data?.length ?? 0
        });
      } catch {
        setSummary({ materiales: 0, proveedores: 0, precios: 0 });
      }
    };
    loadSummary();
  }, []);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-slate-800">Hola, {user?.username} 👋</h1>
        <p className="mt-2 text-slate-500">Este es el panel principal del sistema de ventas.</p>
      </div>
      <div className="grid gap-4 sm:grid-cols-3">
        <StatCard label="Materiales" value={summary.materiales} />
        <StatCard label="Proveedores" value={summary.proveedores} />
        <StatCard label="Precios activos" value={summary.precios} />
      </div>
      <EmptyState message="Selecciona un módulo en la barra lateral para comenzar." />
    </div>
  );
};

const StatCard = ({ label, value }: { label: string; value: number }) => (
  <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
    <p className="text-sm text-slate-500">{label}</p>
    <p className="mt-2 text-3xl font-bold text-slate-800">{value}</p>
  </div>
);
