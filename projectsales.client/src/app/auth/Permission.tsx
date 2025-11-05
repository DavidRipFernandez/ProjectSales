import { useAuth } from './AuthContext';

type PermissionProps = {
  module: string;
  action: string;
  children: React.ReactNode;
  fallback?: React.ReactNode;
};

export const Can = ({ module, action, children, fallback = null }: PermissionProps) => {
  const { hasPermission } = useAuth();
  return hasPermission(module, action) ? <>{children}</> : <>{fallback}</>;
};
