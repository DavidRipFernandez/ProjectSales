import { useEffect, useMemo, useState } from 'react';
import { api } from '../../app/api/axios';
import { Button } from '../../components/Button';
import { useAuth } from '../../app/auth/AuthContext';

interface RoleItem {
  id: number;
  nombre: string;
}

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

interface RolePermissionDto {
  moduleId: number;
  actionId: number;
  allowed: boolean;
}

export const PermissionsPage = () => {
  const { hasPermission } = useAuth();
  const [roles, setRoles] = useState<RoleItem[]>([]);
  const [modules, setModules] = useState<ModuleDto[]>([]);
  const [selectedRole, setSelectedRole] = useState<number | null>(null);
  const [matrix, setMatrix] = useState<Record<string, Record<string, boolean>>>({});
  const [error, setError] = useState<string | null>(null);
  const [status, setStatus] = useState<string | null>(null);

  const canUpdate = hasPermission('Permissions', 'Update');

  useEffect(() => {
    const loadData = async () => {
      try {
        const [rolesResponse, modulesResponse] = await Promise.all([
          api.get('/roles'),
          api.get('/modules')
        ]);
        setRoles(rolesResponse.data?.data ?? []);
        setModules(modulesResponse.data?.data ?? []);
      } catch (err: any) {
        setError(err?.message ?? 'No se pudieron cargar los permisos');
      }
    };
    loadData();
  }, []);

  useEffect(() => {
    if (!selectedRole) return;
    const loadPermissions = async () => {
      try {
        const response = await api.get(`/roles/${selectedRole}/permissions`);
        const permissions: RolePermissionDto[] = response.data?.data ?? [];
        const moduleMap = new Map(modules.map((module) => [module.id, module]));
        const actionMap = new Map<number, ModuleActionDto>();
        modules.forEach((module) => module.actions.forEach((action) => actionMap.set(action.id, action)));

        const nextMatrix: Record<string, Record<string, boolean>> = {};
        permissions.forEach((permission) => {
          const module = moduleMap.get(permission.moduleId);
          const action = actionMap.get(permission.actionId);
          if (module && action) {
            if (!nextMatrix[module.key]) {
              nextMatrix[module.key] = {};
            }
            nextMatrix[module.key][action.key] = permission.allowed;
          }
        });

        setMatrix(nextMatrix);
      } catch (err: any) {
        setError(err?.message ?? 'No se pudieron cargar los permisos del rol');
      }
    };
    loadPermissions();
  }, [selectedRole, modules]);

  const toggle = (moduleKey: string, actionKey: string) => {
    setMatrix((prev) => {
      const modulePermissions = prev[moduleKey] ? { ...prev[moduleKey] } : {};
      modulePermissions[actionKey] = !modulePermissions[actionKey];
      return { ...prev, [moduleKey]: modulePermissions };
    });
  };

  const handleSave = async () => {
    if (!selectedRole) return;
    setStatus(null);
    try {
      const payload = modules.flatMap((module) => module.actions.map((action) => ({
        moduleKey: module.key,
        actionKey: action.key,
        allowed: matrix[module.key]?.[action.key] ?? false
      })));
      await api.put(`/roles/${selectedRole}/permissions`, payload);
      setStatus('Permisos actualizados correctamente.');
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'No se pudieron actualizar los permisos');
    }
  };

  const sortedModules = useMemo(() => modules.map((module) => ({
    ...module,
    actions: [...module.actions].sort((a, b) => a.sortOrder - b.sortOrder)
  })), [modules]);

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-800">Permisos</h2>
        <p className="text-sm text-slate-500">Configura los permisos de cada rol por módulo y acción.</p>
      </div>
      {error && <div className="rounded-md border border-red-200 bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      {status && <div className="rounded-md border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-600">{status}</div>}
      <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
        <div className="mb-4 flex flex-wrap items-center gap-3">
          <label className="text-sm font-medium text-slate-700">
            Seleccionar rol
            <select
              className="ml-3 rounded-md border border-slate-200 px-3 py-2 text-sm focus:border-brand focus:outline-none"
              value={selectedRole ?? ''}
              onChange={(event) => setSelectedRole(event.target.value ? Number(event.target.value) : null)}
            >
              <option value="">Seleccione un rol</option>
              {roles.map((role) => (
                <option key={role.id} value={role.id}>
                  {role.nombre}
                </option>
              ))}
            </select>
          </label>
          {canUpdate && (
            <Button onClick={handleSave} disabled={!selectedRole}>
              Guardar permisos
            </Button>
          )}
        </div>
        {selectedRole ? (
          <div className="space-y-4">
            {sortedModules.map((module) => (
              <div key={module.id} className="rounded-lg border border-slate-100 p-4">
                <h4 className="text-sm font-semibold text-slate-700">{module.name}</h4>
                <div className="mt-3 flex flex-wrap gap-3">
                  {module.actions.map((action) => (
                    <label key={action.id} className="flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm">
                      <input
                        type="checkbox"
                        checked={matrix[module.key]?.[action.key] ?? false}
                        onChange={() => toggle(module.key, action.key)}
                        disabled={!canUpdate}
                      />
                      {action.name}
                    </label>
                  ))}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-sm text-slate-500">Selecciona un rol para editar sus permisos.</p>
        )}
      </div>
    </div>
  );
};
