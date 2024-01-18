import { useNavigate, useSearchParams, useParams } from 'react-router-dom';
import { ToastContainer, Toast, Row } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import ky from 'ky';

type Movie = {
    title: string;
    id: string;
    poster_path: string;
    release_date: string;
};

type Poll = {
    question: string;
    votes: string[];
    movies: Movie[];
    createdAt: Date;
    serverRestricted: boolean;
    expiration: Date | null;
};

export default function Poll() {
    const [searchParams, setSearchParams] = useSearchParams();
    const [error, setError] = useState<string | null>(null);
    const [code, setCode] = useState<string | null>(null);
    const [showError, setShowError] = useState(false);
    const navigate = useNavigate();

    const [poll, setPoll] = useState<Poll | undefined>(undefined);
    const [selected, setSelected] = useState<string | null>(null);
    const { pollId } = useParams();

    useEffect(() => {
        setSearchParams({});

        ky.get(`/api/polls/${pollId}`)
            .json()
            .then((res) => {
                setPoll(res as Poll);
            })
            .catch(async () => {
                setError('Could not get your servers');
                setShowError(true);
                setCode(null);
            });
    }, []);

    async function vote(movieId: string) {
        // try {
        //     await ky.post('/api/polls/', {
        //         json: {
        //         },
        //     });
        // } catch (err) {
        //     setError('Could not create poll.');
        //     setShowError(true);
        // }
    }

    return (
        <div className="container vh-100 vw-100 text-center align-items-center justify-content-center flex">
            <ToastContainer position="top-end">
                <Toast
                    onClose={() => setShowError(false)}
                    show={showError}
                    autohide={true}
                    className="d-inline-block m-1"
                    bg={'danger'}
                >
                    <Toast.Header>
                        <strong className="me-auto">An error occurred!</strong>
                    </Toast.Header>
                    <Toast.Body>{error}</Toast.Body>
                </Toast>
            </ToastContainer>

            <div className="center flex-column align-content-center p-2 justify-content-center align-items-center">
                <Row className="w-50 float-end">
                    <ul className="list-inline-scroll list-unstyled d-flex flex-wrap align-content-center justify-content-center">
                        {poll?.movies.map((movie) => {
                            return (
                                <li>
                                    <img
                                        onClick={() => setSelected(movie.id)}
                                        width="150px"
                                        className="list-inline-item"
                                        src={`https://image.tmdb.org/t/p/original${movie.poster_path}`}
                                        alt="movie poster"
                                        style={{
                                            borderRadius: '5px',
                                            margin: '2px',
                                            cursor: 'pointer',
                                        }}
                                    />
                                    <a href={`https://www.themoviedb.org/movie/${movie.id}`}>
                                        <div style={{ width: '150px' }} className="text-wrap">
                                            {movie.title} {movie.release_date.slice(0, 4)}
                                        </div>
                                    </a>
                                </li>
                            );
                        })}
                    </ul>
                </Row>
            </div>
        </div>
    );
}
