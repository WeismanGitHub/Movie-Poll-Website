import { ToastContainer, Toast, Button, Row, Form, Col, InputGroup, Pagination } from 'react-bootstrap';
import { useNavigate, useSearchParams } from 'react-router-dom';
import DateTimePicker from 'react-datetime-picker';
import { useEffect, useState } from 'react';
import { Field, Formik } from 'formik';
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
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [expirationError, setExpirationError] = useState<string | null>(null);
    const [expirationDate, setExpirationDate] = useState<Date | null>(null);
    const [expires, setExpires] = useState(false);

    const [restrictError, setRestrictError] = useState<string | null>('A server must be selected.');
    const [code, setCode] = useState<string | null>(searchParams.get('code'));
    const [guilds, setGuilds] = useState<Guild[] | null>(null);
    const [guildId, setGuildId] = useState<string | null>();
    const [restricted, setRestricted] = useState(false);

    const [result, setResult] = useState<SearchResult | null>(null);
    const [selected, setSelected] = useState<Movie[]>([]);
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
        ky.get(`/api/search?query=${query}&page=${page}`)
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
            .catch(() => {
                setError('Could not search.');
                setShowError(true);
            });
    }

    useEffect(() => {
        if (restricted && !guildId) {
            setRestrictError('A server must be selected.');
        } else if (!restricted || guildId) {
            setRestrictError(null);
        }
    }, [guildId, restricted]);

    useEffect(() => {
        setSearchParams({});

        if (!code) return;

        setRestricted(true);

        ky.get(`/api/guilds?code=${code}`)
            .json()
            .then((res) => {
                setGuilds(res as Guild[]);
            })
            .catch(async () => {
                setError('Could not get your servers');
                setShowError(true);
                setRestricted(false);
                setCode(null);
            });
    }, []);

    useEffect(() => {
        if (expirationDate && expirationDate.valueOf() <= Date.now()) {
            setExpirationError('Must expire in the future.');
        } else if (!expirationDate && expires) {
            setExpirationError('You must select an expiration date.');
        } else {
            setExpirationError(null);
        }
    }, [expirationDate, expires]);

    const schema = yup.object().shape({
        question: yup
            .string()
            .required('Question is a required field.')
            .min(1, 'Must be at least 1 characters.')
            .max(500, 'Cannot be more than 500 characters.'),
        query: yup.string().min(1).max(500),
    });

    async function createPoll(values: { question: string }) {
        console.log(values, guildId);
        setSelected;
        selected;
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
                <Formik
                    validationSchema={schema}
                    validateOnChange
                    onSubmit={createPoll}
                    initialValues={{
                        question: '',
                        expirationToggle: false,
                        restrictionToggle: false,
                        query: '',
                    }}
                >
                    {({ handleSubmit, handleChange, values, errors, setFieldValue }) => (
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
                                            setExpires(!expires);
                                            setFieldValue('expirationToggle', !values.expirationToggle);
                                            setExpirationDate(null);
                                            setExpirationError(null);
                                        }}
                                    />
                                    <Form.Control hidden={true} isInvalid={Boolean(expirationError)} />
                                    <Form.Control.Feedback type="invalid">
                                        {expirationError}
                                    </Form.Control.Feedback>
                                </Form.Group>
                                <Form.Group as={Col}>
                                    <Form.Label className="me-1">Restricted</Form.Label>
                                    <Field
                                        type="checkbox"
                                        checked={restricted}
                                        name="restrictionToggle"
                                        onChange={() => {
                                            if (!code) {
                                                localStorage.setItem('redirect', 'create');
                                                navigate('/auth');
                                            }

                                            setRestricted(!restricted);
                                            setRestrictError(null);
                                            setFieldValue('restrictionToggle', !values.restrictionToggle);
                                        }}
                                    />
                                    <Form.Control hidden={true} isInvalid={Boolean(restrictError)} />
                                    <Form.Control.Feedback type="invalid">
                                        {restrictError}
                                    </Form.Control.Feedback>
                                </Form.Group>
                            </Row>
                            <Row className="mb-2 d-flex justify-content-center align-content-center">
                                {values.expirationToggle && (
                                    <DateTimePicker
                                        className="w-25 d-flex justify-content-center align-items-center"
                                        onChange={(value) => setExpirationDate(value)}
                                        value={expirationDate}
                                    />
                                )}
                            </Row>
                            <Row className="mb-2">
                                {restricted && guilds && (
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
                                                        guildId === guild.id
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
                                                        setGuildId(guild.id === guildId ? null : guild.id)
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
                                                        `/api/search?query=${values.query}&page=${page}`
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
                            <Row>
                                {!result?.total_results ? (
                                    <div>no results...</div>
                                ) : (
                                    <div>
                                        <Col className="w-50 float-start">
                                            <ul className="list-inline-scroll list-unstyled d-flex flex-wrap align-content-center justify-content-center">
                                                {result.results.map((movie) => {
                                                    return (
                                                        <li>
                                                            <img
                                                                onClick={() => {
                                                                    if (selected.length === 50) {
                                                                        setError('Cannot add more than 50 movies.');
                                                                        setShowError(true);
                                                                        window.scrollTo(0, 0)
                                                                        return
                                                                    }

                                                                    setSelected((s) => [
                                                                        ...new Set([...s, movie]),
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
                                                {selected.map((movie) => {
                                                    return (
                                                        <li>
                                                            <img
                                                                onClick={() =>
                                                                    setSelected((s) =>
                                                                        s.filter((m) => m.id !== movie.id)
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
                                )}
                            </Row>
                            <Button type="submit">Create</Button>
                        </Form>
                    )}
                </Formik>
            </div>
        </div>
    );
}
