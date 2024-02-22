import { ToastContainer, Toast, Button, Row, Form, Col, InputGroup, Pagination } from 'react-bootstrap';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { Field, Formik } from 'formik';
import NavBar from '../navbar';
import * as yup from 'yup';
import ky from 'ky';

type Movie = {
    title: string;
    id: string;
    poster_path: string;
    release_date: string;
};

type SearchResult = {
    page: number;
    total_pages: number;
    total_results: number;
    results: Movie[];
};

export default function CreatePoll() {
    const [searchParams, setSearchParams] = useSearchParams();
    const [error, setError] = useState<string | null>(null);
    const [showError, setShowError] = useState(false);
    const code = searchParams.get('code');
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [token, setToken] = useState<string | null>(null);
    const [guilds, setGuilds] = useState<Guild[] | null>(null);

    const [result, setResult] = useState<SearchResult | null>(null);
    const [query, setQuery] = useState('');

    useEffect(() => {
        setPage(1);
    }, [query]);

    useEffect(() => {
        if (query.length) {
            search();
        }
    }, [page]);

    async function search() {
        ky.get(`/api/tmdb/search?query=${query}&page=${page}`)
            .then(async (res) => {
                const searchRes: SearchResult = await res.json();

                if (
                    searchRes.total_results == undefined ||
                    searchRes.results == undefined ||
                    searchRes.total_pages == undefined ||
                    searchRes.page == undefined
                ) {
                    setError('Could not search.');
                    return setShowError(true);
                }

                setResult(searchRes);
            })
            .catch((err) => {
                console.log(err);
                setError('Could not search.');
                setShowError(true);
            });
    }

    useEffect(() => {
        setSearchParams({});

        if (!code) return;

        (async function () {
            try {
                const token: string = await ky.get(`/api/discord/token?code=${code}`).json();
                const guilds: Guild[] = await ky.get(`/api/discord/guilds?token=${token!}`).json();
                setToken(token);
                setGuilds(guilds);
            } catch (err) {
                console.log(err);
                setError('Could not get your servers');
                setShowError(true);
            }
        })();
    }, []);

    const schema = yup.object().shape({
        question: yup
            .string()
            .required('Question is a required field.')
            .min(1, 'Must be at least 1 characters.')
            .max(500, 'Cannot be more than 500 characters.'),
        query: yup
            .string()
            .min(1, 'Query must be at least 1 character.')
            .max(500, 'Query cannot be more than 500 characters.'),
        expirationDate: yup.date().nullable().min(new Date(), 'Expiration must be in the future.'),
        guildId: yup.string().nullable(),
        movies: yup
            .array<Movie>()
            .required('You must select movies.')
            .min(2, 'You must select at least 2 movies.')
            .max(50, 'You cannot select more than 50 movies.'),
    });

    return (
        <div className='vh-100 vw-100'>
            <NavBar />
            <div className="container text-center align-items-center justify-content-center flex">
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
                    <Formik
                        validationSchema={schema}
                        validateOnChange
                        onSubmit={async (values) => {
                            try {
                                var id = await ky
                                    .post('/api/polls/', {
                                        json: {
                                            question: values.question,
                                            expiration: values.expirationDate,
                                            guildId: values.restrictionToggle ? values.guildId : null,
                                            accessToken: values.restrictionToggle ? token : null,
                                            movieIds: values.movies.map((movie) => String(movie.id)),
                                        },
                                    })
                                    .json();

                                navigate(`/polls/${id}`);
                            } catch (err) {
                                window.scrollTo(0, 0);
                                setError('Could not create poll.');
                                setShowError(true);
                            }
                        }}
                        initialValues={{
                            question: '',
                            expirationToggle: false,
                            restrictionToggle: Boolean(code),
                            query: '',
                            expirationDate: null,
                            guildId: null,
                            movies: [] as Movie[],
                        }}
                        validate={(values) => {
                            const errors: {
                                expirationDate?: string;
                                guildId?: string;
                            } = {};

                            if (values.movies.length > 50) {
                                window.scrollTo(0, 0);
                            }

                            if (values.expirationToggle && !values.expirationDate) {
                                errors.expirationDate = 'Choose an expiration.';
                            }

                            if (values.restrictionToggle && !values.guildId) {
                                errors.guildId = 'Choose a server.';
                            }

                            return errors;
                        }}
                    >
                        {({ handleSubmit, handleChange, values, errors, setFieldValue, setFieldError }) => {
                            return (
                                <Form noValidate onSubmit={handleSubmit}>
                                    <Row className="mb-2">
                                        <Form.Group as={Col}>
                                            <Form.Label>Question</Form.Label>
                                            <Form.Control
                                                as="textarea"
                                                type="textarea"
                                                rows={4}
                                                name="question"
                                                value={values.question}
                                                onChange={handleChange}
                                                isInvalid={!!errors.question}
                                            />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.question}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Row>
                                    <Row className="mb-2">
                                        <Form.Group as={Col}>
                                            <Form.Label className="me-1">Expires</Form.Label>
                                            <Field
                                                type="checkbox"
                                                name="expirationToggle"
                                                onChange={() => {
                                                    setFieldValue('expirationToggle', !values.expirationToggle);
                                                    setFieldValue('expirationDate', null);
                                                }}
                                            />
                                            <Form.Control
                                                hidden={true}
                                                isInvalid={Boolean(errors.expirationDate)}
                                            />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.expirationDate}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                        <Form.Group as={Col}>
                                            <Form.Label className="me-1">Server Restricted</Form.Label>
                                            <Field
                                                type="checkbox"
                                                checked={values.restrictionToggle}
                                                name="restrictionToggle"
                                                onChange={() => {
                                                    if (token) {
                                                        setFieldValue(
                                                            'restrictionToggle',
                                                            !values.restrictionToggle
                                                        );
                                                        return setFieldValue('guildId', null);
                                                    } else if (
                                                        confirm(
                                                            'This will redirect you and you will lose all inputted data.'
                                                        )
                                                    ) {
                                                        localStorage.setItem('redirect', 'create');
                                                        return navigate('/auth');
                                                    }
                                                }}
                                            />
                                            <Form.Control hidden={true} isInvalid={Boolean(errors.guildId)} />
                                            <Form.Control.Feedback type="invalid">
                                                {errors.guildId}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                    </Row>
                                    <Row className="mb-2 d-flex justify-content-center align-content-center">
                                        {values.expirationToggle && (
                                            <input
                                                type="datetime-local"
                                                className="d-flex justify-content-center align-items-center"
                                                style={{ minWidth: '100px', maxWidth: '350px' }}
                                                value={values.expirationDate ?? undefined}
                                                min={Date.now()}
                                                onChange={(value) => {
                                                    setFieldValue('expirationDate', value.target.value);
                                                }}
                                            />
                                        )}
                                    </Row>
                                    <Row className="mb-2">
                                        {values.restrictionToggle && guilds && (
                                            <ul
                                                className="list-inline-scroll list-unstyled d-flex overflow-x-scroll overflow-y-hidden"
                                                style={{ height: '90px' }}
                                            >
                                                {guilds.length ? (
                                                    guilds.map((guild) => (
                                                        <li
                                                            key={guild.id}
                                                            className="list-inline-item d-flex brightness-effect"
                                                            style={
                                                                values.guildId === guild.id
                                                                    ? {
                                                                        border: 'solid black 2px',
                                                                        borderRadius: '5px',
                                                                        padding: '2px',
                                                                    }
                                                                    : {
                                                                        border: 'solid transparent 2px',
                                                                        borderRadius: '5px',
                                                                        padding: '2px',
                                                                    }
                                                            }
                                                            onClick={() =>
                                                                setFieldValue(
                                                                    'guildId',
                                                                    guild.id === values.guildId ? null : guild.id
                                                                )
                                                            }
                                                        >
                                                            <img
                                                                width={65}
                                                                height={65}
                                                                src={
                                                                    guild.icon
                                                                        ? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png`
                                                                        : '/discord.svg'
                                                                }
                                                                alt="server name"
                                                                style={{
                                                                    borderRadius: '50%',
                                                                    marginRight: '2px',
                                                                }}
                                                            />
                                                            <div
                                                                className="text-nowrap flex-grow-1 d-flex align-items-center justify-content-center"
                                                                style={{ height: '65px' }}
                                                            >
                                                                {guild.name}
                                                            </div>
                                                        </li>
                                                    ))
                                                ) : (
                                                    <div className="text-danger justify-content-center align-content-center w-100">
                                                        <u>no servers...</u>
                                                    </div>
                                                )}
                                            </ul>
                                        )}
                                    </Row>
                                    <Row className="mb-2">
                                        <Form.Group as={Col} controlId="query">
                                            <Form.Label>Search Movies</Form.Label>
                                            <InputGroup hasValidation>
                                                <Form.Control
                                                    type="text"
                                                    aria-describedby="inputGroupPrepend"
                                                    name="query"
                                                    value={values.query}
                                                    onChange={(change) => {
                                                        setQuery(change.target.value);
                                                        handleChange(change);
                                                    }}
                                                    isInvalid={!!errors.query}
                                                />
                                                <Button
                                                    onClick={async () => {
                                                        try {
                                                            const res = await ky.get(
                                                                `/api/tmdb/search?query=${values.query}&page=${page}`
                                                            );
                                                            const searchRes: SearchResult = await res.json();

                                                            if (
                                                                searchRes.total_results == undefined ||
                                                                searchRes.results == undefined ||
                                                                searchRes.total_pages == undefined ||
                                                                searchRes.page == undefined
                                                            ) {
                                                                setError('Could not search.');
                                                                return setShowError(true);
                                                            }

                                                            setResult(searchRes);

                                                            if (searchRes.total_results === 0) {
                                                                setFieldError('query', 'no results');
                                                            }
                                                        } catch (err) {
                                                            setError('Could not search.');
                                                            setShowError(true);
                                                        }
                                                    }}
                                                >
                                                    Search
                                                </Button>
                                                <Form.Control.Feedback type="invalid">
                                                    {errors.query}
                                                </Form.Control.Feedback>
                                            </InputGroup>
                                        </Form.Group>
                                    </Row>
                                    <Row>
                                        <Pagination size="sm" className="justify-content-center">
                                            <Pagination.First
                                                disabled={!query.length || page === 1}
                                                onClick={() => {
                                                    setPage(1);
                                                }}
                                            />
                                            <Pagination.Prev
                                                disabled={!query.length || page === 1}
                                                onClick={() => {
                                                    setPage(page - 1);
                                                }}
                                            />
                                            <Pagination.Item>{page}</Pagination.Item>
                                            <Pagination.Next
                                                disabled={!query.length || result?.total_pages === page}
                                                onClick={() => {
                                                    setPage(page + 1);
                                                }}
                                            />
                                            <Pagination.Last
                                                disabled={
                                                    !query.length ||
                                                    !result?.total_pages ||
                                                    result?.total_pages === page
                                                }
                                                onClick={() => {
                                                    setPage(result!.total_pages);
                                                }}
                                            />
                                        </Pagination>
                                    </Row>
                                    <Button type="submit">Create</Button>
                                    <Form.Group controlId="movies">
                                        <InputGroup hasValidation>
                                            <Form.Control hidden isInvalid={!!errors.movies} name="movies" />
                                            <Form.Control.Feedback type="invalid">
                                                {/* @ts-ignore */}
                                                {errors.movies}
                                            </Form.Control.Feedback>
                                        </InputGroup>
                                    </Form.Group>
                                    <Row>
                                        <div>
                                            <Col className="w-50 float-start">
                                                <ul className="list-inline-scroll list-unstyled d-flex flex-wrap align-content-center justify-content-center">
                                                    {result?.results.map((movie) => {
                                                        return (
                                                            <li>
                                                                <img
                                                                    onClick={() => {
                                                                        setFieldValue('movies', [
                                                                            ...new Set([...values.movies, movie]),
                                                                        ]);
                                                                    }}
                                                                    width="150px"
                                                                    className="list-inline-item"
                                                                    src={`https://image.tmdb.org/t/p/original${movie.poster_path}`}
                                                                    alt="movie poster"
                                                                    style={{
                                                                        cursor: 'pointer',
                                                                        borderRadius: '5px',
                                                                        margin: '2px',
                                                                    }}
                                                                />
                                                                <a
                                                                    href={`https://www.themoviedb.org/movie/${movie.id}`}
                                                                >
                                                                    <div
                                                                        style={{ width: '150px' }}
                                                                        className="text-wrap"
                                                                    >
                                                                        {movie.title}{' '}
                                                                        {movie.release_date.slice(0, 4)}
                                                                    </div>
                                                                </a>
                                                            </li>
                                                        );
                                                    })}
                                                </ul>
                                            </Col>
                                            <Col className="w-50 float-end">
                                                <ul className="list-inline-scroll list-unstyled d-flex flex-wrap align-content-center justify-content-center">
                                                    {values.movies.map((movie) => {
                                                        return (
                                                            <li>
                                                                <img
                                                                    onClick={() =>
                                                                        setFieldValue(
                                                                            'movies',
                                                                            values.movies.filter(
                                                                                (m) => m.id !== movie.id
                                                                            )
                                                                        )
                                                                    }
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
                                                                <a
                                                                    href={`https://www.themoviedb.org/movie/${movie.id}`}
                                                                >
                                                                    <div
                                                                        style={{ width: '150px' }}
                                                                        className="text-wrap"
                                                                    >
                                                                        {movie.title}{' '}
                                                                        {movie.release_date.slice(0, 4)}
                                                                    </div>
                                                                </a>
                                                            </li>
                                                        );
                                                    })}
                                                </ul>
                                            </Col>
                                        </div>
                                    </Row>
                                </Form>
                            );
                        }}
                    </Formik>
                </div>
            </div>
        </div>
    );
}
