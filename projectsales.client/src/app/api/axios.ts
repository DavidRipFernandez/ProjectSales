import axios from 'axios';

const API_BASE_URL = 'https://localhost:7030';

type AuthHandlers = {
  getAccessToken: () => string | null;
  setAccessToken: (token: string) => void;
  logout: () => Promise<void> | void;
};

let handlers: AuthHandlers = {
  getAccessToken: () => null,
  setAccessToken: () => {},
  logout: () => {}
};

export const registerAuthHandlers = (nextHandlers: Partial<AuthHandlers>) => {
  handlers = { ...handlers, ...nextHandlers } as AuthHandlers;
};

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true
});

let refreshPromise: Promise<string | null> | null = null;

const refreshAccessToken = async (): Promise<string | null> => {
  try {
    const response = await axios.post(
      '/auth/refresh',
      null,
      {
        baseURL: API_BASE_URL,
        withCredentials: true,
        headers: { Authorization: undefined }
      }
    );

    const data = response.data?.data ?? response.data;
    const token = data?.accessToken ?? data?.AccessToken ?? null;
    if (token) {
      handlers.setAccessToken(token);
    }
    return token ?? null;
  } catch (error) {
    await handlers.logout();
    return null;
  }
};

api.interceptors.request.use((config) => {
  const token = handlers.getAccessToken();
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const status = error.response?.status;
    const originalRequest = error.config;

    if (status === 401 && !originalRequest?._retry) {
      if (!refreshPromise) {
        refreshPromise = refreshAccessToken().finally(() => {
          refreshPromise = null;
        });
      }

      const newToken = await refreshPromise;
      if (newToken) {
        originalRequest._retry = true;
        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers.Authorization = `Bearer ${newToken}`;
        return api(originalRequest);
      }
    }

    return Promise.reject(error);
  }
);

export { api };
