import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { api, registerAuthHandlers } from '../api/axios';

type PermissionsByModule = Record<string, string[]>;

type AuthUser = {
  id: number;
  username: string;
  email: string;
};

type AuthContextValue = {
  user: AuthUser | null;
  roles: string[];
  permissions: PermissionsByModule;
  accessToken: string;
  loading: boolean;
  login: (usernameOrEmail: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  fetchMe: () => Promise<void>;
  hasPermission: (moduleKey: string, actionKey: string) => boolean;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const normalizeResponse = (response: any) => response?.data ?? response;

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [roles, setRoles] = useState<string[]>([]);
  const [permissions, setPermissions] = useState<PermissionsByModule>({});
  const [accessToken, setAccessToken] = useState('');
  const [loading, setLoading] = useState(true);
  const accessTokenRef = useRef('');
  const logoutRef = useRef<() => Promise<void>>(async () => {});

  const setAccessTokenState = useCallback((token: string) => {
    accessTokenRef.current = token;
    setAccessToken(token);
  }, []);

  const applyAuthPayload = useCallback((payload: any) => {
    if (!payload) return;
    const summary = payload.user ?? payload.User;
    if (summary) {
      setUser({ id: summary.id, username: summary.username, email: summary.email });
    }
    const payloadRoles = payload.roles ?? payload.Roles ?? [];
    const payloadPermissions = payload.permissionsByModule ?? payload.PermissionsByModule ?? {};
    setRoles(payloadRoles);
    setPermissions(payloadPermissions);
  }, []);

  const fetchMe = useCallback(async () => {
    try {
      setLoading(true);
      const response = await api.get('/auth/me');
      const data = normalizeResponse(response.data);
      if (data?.data) {
        applyAuthPayload(data.data);
      }
    } catch (error) {
      setUser(null);
      setRoles([]);
      setPermissions({});
      setAccessTokenState('');
    } finally {
      setLoading(false);
    }
  }, [applyAuthPayload, setAccessTokenState]);

  const login = useCallback(async (usernameOrEmail: string, password: string) => {
    const response = await api.post('/auth/login', { usernameOrEmail, password });
    const data = normalizeResponse(response.data);
    const payload = data?.data ?? data;
    if (!payload?.accessToken) {
      throw new Error('No se pudo iniciar sesión');
    }
    setAccessTokenState(payload.accessToken);
    applyAuthPayload(payload);
  }, [applyAuthPayload, setAccessTokenState]);

  const logout = useCallback(async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      // ignore network errors on logout
    } finally {
      setAccessTokenState('');
      setUser(null);
      setRoles([]);
      setPermissions({});
    }
  }, [setAccessTokenState]);

  useEffect(() => {
    logoutRef.current = logout;
  }, [logout]);

  useEffect(() => {
    registerAuthHandlers({
      getAccessToken: () => accessTokenRef.current,
      setAccessToken: setAccessTokenState,
      logout: () => logoutRef.current()
    });
  }, [setAccessTokenState]);

  useEffect(() => {
    fetchMe();
  }, [fetchMe]);

  const hasPermission = useCallback((moduleKey: string, actionKey: string) => {
    const modulePermissions = permissions[moduleKey] ?? [];
    if (modulePermissions.includes(actionKey)) {
      return true;
    }
    return roles.includes('Master');
  }, [permissions, roles]);

  const value = useMemo<AuthContextValue>(() => ({
    user,
    roles,
    permissions,
    accessToken,
    loading,
    login,
    logout,
    fetchMe,
    hasPermission
  }), [user, roles, permissions, accessToken, loading, login, logout, fetchMe, hasPermission]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextValue => {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth debe utilizarse dentro de AuthProvider');
  }
  return ctx;
};
