
import React, { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import './index.css';
import { AuthProvider } from './app/auth/AuthContext';
import { router } from './app/router';

const rootEl = document.getElementById('root');
if (!rootEl) {
    throw new Error('Elemento #root no encontrado en index.html');
}

createRoot(rootEl).render(
    <StrictMode>
        <AuthProvider>
            <RouterProvider router={router} />
        </AuthProvider>
    </StrictMode>
);
