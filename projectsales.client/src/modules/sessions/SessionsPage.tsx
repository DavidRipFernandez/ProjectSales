import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';
import { Button } from '../../components/Button';

interface SessionDto {
  id: number;
  createdAt: string;
  lastSeenAt?: string;
  expiresAt: string;
  revoked: boolean;
  ipAddress?: string;
  userAgent?: string;
  deviceName?: string;
  location?: string;
}

export const SessionsPage = () => {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [error, setError] = useState<string | null>(null);

  const loadSessions = async () => {
    const response = await api.get('/sessions/mine');
    setSessions(response.data?.data ?? []);
  };

  useEffect(() => {
    loadSessions().catch(() => setError('No se pudieron cargar las sesiones'));
  }, []);

  const handleRevoke = async (sessionId: number) => {
    if (!confirm('¿Deseas revocar esta sesión?')) return;
    try {
      await api.post(`/sessions/${sessionId}/revoke`);
      await loadSessions();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudo revocar la sesión');
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Mis sesiones</h2>
        <p className="text-sm text-slate-500">Consulta y revoca sesiones activas en otros dispositivos.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      <div className="grid gap-4">
        {sessions.map((session) => (
          <div key={session.id} className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <div className="flex flex-wrap items-center justify-between gap-4">
              <div>
                <p className="text-sm font-semibold text-slate-700">Creada: {new Date(session.createdAt).toLocaleString()}</p>
                <p className="text-xs text-slate-500">Último acceso: {session.lastSeenAt ? new Date(session.lastSeenAt).toLocaleString() : '—'}</p>
                <p className="text-xs text-slate-500">Expira: {new Date(session.expiresAt).toLocaleString()}</p>
                <p className="text-xs text-slate-500">IP: {session.ipAddress || '—'} · Dispositivo: {session.deviceName || '—'}</p>
              </div>
              <Button variant="ghost" onClick={() => handleRevoke(session.id)} disabled={session.revoked}>
                {session.revoked ? 'Revocada' : 'Revocar'}
              </Button>
            </div>
          </div>
        ))}
        {sessions.length === 0 && <p className="text-sm text-slate-500">No hay sesiones activas.</p>}
      </div>
    </div>
  );
};
