import { createBrowserRouter } from 'react-router-dom';

import CreatePoll from './pages/create-poll.tsx';
import Home from './pages/home.tsx';
import Auth from './pages/auth.tsx';
import Poll from './pages/poll.tsx';

const router = createBrowserRouter([
    { path: '/polls/:pollId', element: <Poll /> },
    { path: '/create/', element: <CreatePoll /> },
    { path: '/auth', element: <Auth /> },
    { path: '/', element: <Home /> },
]);

export default router;
