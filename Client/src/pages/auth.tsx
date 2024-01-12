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
        const redirect = searchParams.get('redirect');
        const state = searchParams.get('state');
        const code = searchParams.get('code');

        if (!redirect) {
            navigate('/')
        }

        if (code && state && localStorage.getItem('auth-state') == atob(decodeURIComponent(state))) {
            navigate(`/${redirect}?code=${code}`)
        } else {
            localStorage.setItem('auth-state', randomString);
        }
    }, []);

    return (
        <>
            <a
                className=""
                href={process.env.REACT_APP_OAUTH_URL + `&state=${btoa(randomString)}`}
            >
                Authorize
            </a>
        </>
    );
}
