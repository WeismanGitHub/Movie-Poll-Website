import { useSearchParams, useParams, useNavigate } from 'react-router-dom';
import { ToastContainer, Toast, Button, Modal } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import ky from 'ky';

type Movie = {
    title: string;
    id: string;
    posterPath: string;
    releaseDate: string;
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
    const [token, setToken] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [showError, setShowError] = useState(false);

    const [voteMap, setVoteMap] = useState<Map<string, number> | null>(new Map<string, number>());
    const [poll, setPoll] = useState<Poll | undefined>(undefined);
    const [selected, setSelected] = useState<Movie | null>(null);
    const { pollId } = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        const code = searchParams.get('code')
        setSearchParams({});

        if (code) {
            ky.get(`/api/discord/token?code=${code}`).json()
            .then(res => setToken(res as string))
            .catch(async (err) => {
                console.log(err);
                setError('Could not get access token.');
                setShowError(true);
            });
        }

        ky.get(`/api/polls/${pollId}`)
            .json()
            .then((res) => {
                setPoll(res as Poll);
                const map = new Map<string, number>();
                (res as Poll).votes.forEach((id) => {
                    const votes = map.get(id);
                    map.set(id, votes == undefined ? 1 : votes + 1);
                });
                setVoteMap(map)
            })
            .catch(async (err) => {
                console.log(err);
                setError('Could not get poll.');
                setShowError(true);
            });
    }, []);

    async function vote() {
        try {
            await ky.post(`/api/polls/${pollId}/vote`, {
                json: {
                    accessToken: token,
                    movieId: selected!.id
                },
            });

            setSelected(null)
        } catch (err) {
            console.log(err)
            setError('Could not vote.');
            setShowError(true);
        }
    }

    return (
        <div className="container d-flex text-break vw-100 align-items-center justify-content-center flex-column text-center">
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

            <Modal show={selected !== null}>
                <Modal.Dialog>
                    <Modal.Header closeButton onClick={() => setSelected(null)}></Modal.Header>
                    <Modal.Body className="d-flex align-items-center justify-content-center text-center">
                        <div>
                            <img
                                width="150px"
                                className="list-inline-item"
                                src={`https://image.tmdb.org/t/p/original${selected?.posterPath}`}
                                alt="movie poster"
                                style={{
                                    borderRadius: '5px',
                                    margin: '2px',
                                }}
                            />
                            <a href={`https://www.themoviedb.org/movie/${selected?.id}`}>
                                <div style={{ width: '150px' }} className="text-wrap">
                                    {selected?.title} {selected?.releaseDate.slice(0, 4)}
                                </div>
                            </a>
                            <div>
                                <Button onClick={vote}>Vote</Button>
                            </div>
                        </div>
                    </Modal.Body>
                </Modal.Dialog>
            </Modal>

            <div style={{ width: '90%' }}>
                {poll ? (
                    <div className="m-auto rounded shadow p-2">
                        <div className="fs-3">{poll.question}</div>
                        {poll.serverRestricted && (
                            <div
                                className="border-1 border rounded bg-dark-subtle d-inline-block m-1"
                                style={{ padding: '2px', fontSize: '12px' }}
                            >
                                Server Restricted
                            </div>
                        )}
                        <div className="d-flex justify-content-center" style={{ fontSize: '12px' }}>
                            <div
                                className="border-1 border rounded bg-dark-subtle d-inline-block m-1"
                                style={{ padding: '2px' }}
                            >
                                {`Created ${new Date(poll.createdAt).toLocaleDateString('en-US', {
                                    weekday: 'long',
                                    year: 'numeric',
                                    month: 'long',
                                    day: 'numeric',
                                })}`}
                            </div>
                            {poll.expiration && (
                                <div
                                    className="border-1 border rounded bg-dark-subtle d-inline-block m-1"
                                    style={{ padding: '2px' }}
                                >
                                    Expires{' '}
                                    {new Date(poll.expiration).toLocaleDateString('en-US', {
                                        weekday: 'long',
                                        year: 'numeric',
                                        month: 'long',
                                        day: 'numeric',
                                    })}
                                </div>
                            )}
                        </div>
                        <br />
                        {token ? (
                            <div>Click on a movie to vote!</div>
                        ) : (
                            <Button
                                onClick={() => {
                                    localStorage.setItem('redirect', `polls/${pollId}`);
                                    navigate('/auth');
                                }}
                            >
                                Login to Vote
                            </Button>
                        )}
                    </div>
                ) : (
                    'loading...'
                )}
                <br />
                    <ul className="list-inline-scroll list-unstyled d-flex flex-wrap align-content-center justify-content-center">
                        {(poll && voteMap) && 
                            poll.movies.sort((a, b) => voteMap.get(a.id)! - voteMap.get(b.id)!)
                            .map((movie) => {
                                const votes = voteMap.get(movie.id)
                                return (
                                    <li>
                                        <img
                                            onClick={() => {
                                                token ? setSelected(movie) : null;
                                            }}
                                            width="150px"
                                            className="list-inline-item"
                                            src={`https://image.tmdb.org/t/p/original${movie.posterPath}`}
                                            alt="movie poster"
                                            style={{
                                                borderRadius: '5px',
                                                margin: '2px',
                                                cursor: token ? 'pointer' : 'default',
                                            }}
                                        />
                                        <a href={`https://www.themoviedb.org/movie/${movie.id}`}>
                                            <div style={{ width: '150px' }} className="text-wrap">
                                                {movie.title} {movie.releaseDate.slice(0, 4)}
                                            </div>
                                        </a>
                                        {votes ?? 0} vote{votes == 1 ? '' : 's'}
                                    </li>
                                );
                            })}
                    </ul>
            </div>
        </div>
    );
}
