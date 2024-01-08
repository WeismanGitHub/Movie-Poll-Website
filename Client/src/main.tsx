import { RouterProvider } from 'react-router-dom';
import ReactDOM from 'react-dom/client';
import router from './router';
import React from 'react';

import 'react-datetime-picker/dist/DateTimePicker.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-calendar/dist/Calendar.css';
import 'react-clock/dist/Clock.css';

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <RouterProvider router={router} />
    </React.StrictMode>
);
