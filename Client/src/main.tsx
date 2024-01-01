import { RouterProvider } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import ReactDOM from 'react-dom/client';
import router from './router';
import React from 'react';
import '../theme.css';
import './style.css';

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
          <RouterProvider router={router} />
    </React.StrictMode>
);
