import { useSearchParams, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

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
            navigate('/')
            return;
        }

        if (code && state && localStorage.getItem('auth-state') == atob(decodeURIComponent(state)) && redirect) {
            navigate(`/${redirect}?code=${code}`)
        } else {
            localStorage.setItem('auth-state', randomString);
            localStorage.setItem('redirect', redirect);
        }
    }, []);

    return (
        <>
            <a
                className=""
                href={import.meta.env.VITE_OAUTH_URL + `&state=${btoa(randomString)}`}
            >
                Authorize
            </a>
        </>
    );
}
