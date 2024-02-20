import { useSearchParams, useNavigate } from 'react-router-dom';
import { Button } from 'react-bootstrap';
import { useEffect } from 'react';
import Navbar from '../navbar';

function generateState() {
    const randomNumber = Math.floor(Math.random() * 10);
    let randomString = '';

    for (let i = 0; i < 20 + randomNumber; i++) {
        randomString += String.fromCharCode(33 + Math.floor(Math.random() * 94));
    }

    return randomString;
}

export default function Auth() {
    const [searchParams] = useSearchParams();
    const randomString = generateState();
    const navigate = useNavigate();

    useEffect(() => {
        const state = searchParams.get('state');
        const code = searchParams.get('code');
        const redirect = localStorage.getItem('redirect');

        if (!redirect) {
            navigate('/');
            return;
        }

        if (
            code &&
            state &&
            localStorage.getItem('auth-state') == atob(decodeURIComponent(state)) &&
            redirect
        ) {
            navigate(`/${redirect}?code=${code}`);
        } else {
            localStorage.setItem('auth-state', randomString);
            localStorage.setItem('redirect', redirect);
        }
    }, []);

    return (
        <>
            <Navbar />
            <div className="container d-flex vh-100 vw-100 align-items-center justify-content-center flex-column text-center">
                <Button
                    href={import.meta.env.VITE_OAUTH_URL + `&state=${btoa(randomString)}`}
                    className="btn-lg"
                >
                    Authorize
                </Button>
            </div>
        </>
    );
}
