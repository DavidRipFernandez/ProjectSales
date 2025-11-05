import { useEffect, useState } from 'react';
import { api } from '../../app/api/axios';

interface ModuleActionDto {
  id: number;
  key: string;
  name: string;
  sortOrder: number;
  isActive: boolean;
}

interface ModuleDto {
  id: number;
  key: string;
  name: string;
  isActive: boolean;
  actions: ModuleActionDto[];
}

export const ModulesPage = () => {
  const [modules, setModules] = useState<ModuleDto[]>([]);

  useEffect(() => {
    const load = async () => {
      const response = await api.get('/modules');
      setModules(response.data?.data ?? []);
    };
    load();
  }, []);

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Módulos</h2>
        <p className="text-sm text-slate-500">Listado de módulos y acciones disponibles en la aplicación.</p>
      </div>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {modules.map((module) => (
          <div key={module.id} className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <h3 className="text-lg font-semibold text-slate-700">{module.name}</h3>
            <p className="text-xs uppercase text-slate-400">{module.key}</p>
            <div className="mt-3 space-y-2">
              {module.actions
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((action) => (
                  <div key={action.id} className="flex items-center justify-between rounded-lg border border-slate-100 bg-slate-50 px-3 py-2 text-sm">
                    <span className="font-medium text-slate-700">{action.name}</span>
                    <span className="text-xs uppercase text-slate-400">{action.key}</span>
                  </div>
                ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
