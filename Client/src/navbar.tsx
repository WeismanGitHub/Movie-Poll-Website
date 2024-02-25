import { useState } from 'react';

export default function NavBar() {
    const [isNavOpen, setIsNavOpen] = useState(false);

    return (
        <>
            <nav
                className="navbar navbar-expand-md py-1 ps-2 pe-2"
                style={{ textAlign: 'center', fontSize: 'x-large', backgroundColor: '#9CC3FF' }}
            >
                <div className="container-fluid">
                    <a className="navbar-brand" href="/">
                        <img
                            src="/icon.svg"
                            width="50"
                            height="50"
                            alt="icon"
                            className="me-2 rounded-5 bg-white"
                            style={{
                                borderRadius: '100%',
                                borderColor: 'black',
                                borderWidth: '1px',
                                borderStyle: 'solid',
                            }}
                        />
                        <span className="d-block d-sm-inline-block">Movie Polls</span>
                    </a>
                    <button
                        className="navbar-toggler"
                        type="button"
                        onClick={() => setIsNavOpen(!isNavOpen)}
                        aria-controls="navbarNav"
                        aria-expanded={isNavOpen ? 'true' : 'false'}
                        aria-label="Toggle navigation"
                    >
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div
                        className={`justify-content-end collapse navbar-collapse${isNavOpen ? ' show' : ''}`}
                        id="navbarNav"
                    >
                        <ul className="navbar-nav d-flex justify-content-center align-items-center">
                            <li className={`nav-item nav-link active w-75 m-1 ${isNavOpen ? ' mb-2' : ''}`}>
                                <a href="/create" style={{ textDecoration: 'none', color: 'white' }}>
                                    Create
                                </a>
                            </li>
                            <li className={`nav-item nav-link active w-75 m-1 ${isNavOpen ? ' mb-2' : ''}`}>
                                <a
                                    href="https://github.com/WeismanGitHub/Movie-Poll-Website"
                                    style={{ textDecoration: 'none', color: 'white' }}
                                >
                                    GitHub
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
        </>
    );
}
