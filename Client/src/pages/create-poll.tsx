import { ToastContainer, Toast, Button, Row, Form, Col, InputGroup } from 'react-bootstrap';
import { useNavigate, useSearchParams } from 'react-router-dom';
import DateTimePicker from 'react-datetime-picker';
import { useEffect, useState } from 'react';
import { Field, Formik } from 'formik';
import * as yup from 'yup';
import ky from 'ky';

export default function CreatePoll() {
    const [error, setError] = useState<string | null>(null);
    const [showError, setShowError] = useState(false);
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const [expirationError, setExpirationError] = useState<string | null>(null);
    const [expirationDate, setExpirationDate] = useState<Date | null>(null);
    const [expires, setExpires] = useState(false)

    const [restrictError, setRestrictError] = useState<string | null>("A guild must be selected.");
    const [code, setCode] = useState<string | null>(searchParams.get('code'));
    const [guilds, setGuilds] = useState<Guild[] | null>(null);
    const [guildId, setGuildId] = useState<string | null>();
    const [restricted, setRestricted] = useState(false);

    // const [page, setPage] = useState(0);

    useEffect(() => {
        if (restricted && !guildId) {
            setRestrictError("A guild must be selected.")
        } else if (!restricted) {
            setRestrictError(null)
        }
    }, [guildId, restricted])

    useEffect(() => {
        searchParams.delete('code')

        if (!code) return;

        setRestricted(true);

        ky.get(`/api/self/guilds?code=${code}`)
            .json()
            .then((res) => {
                setGuilds(res as Guild[]);
            })
            .catch(() => {
                setError('Could not get your servers.');
                setShowError(true);
                setRestricted(false);
                setCode(null);
            });
    }, []);

    useEffect(() => {
        if (expirationDate && expirationDate.valueOf() <= Date.now()) {
            setExpirationError('Must expire in the future.');
        } else if (!expirationDate && expires) {
            setExpirationError("You must select an expiration date.")
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
    });

    async function createPoll(values: { question: string }) {
        console.log(values, guildId);
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
                        search: '',
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
                                            setExpires(!expires)
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
                            <Row className='mb-2 d-flex justify-content-center align-content-center'>
                                {values.expirationToggle && (
                                    <DateTimePicker
                                        className='w-25'
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
                                                    className="list-inline-item d-flex"
                                                    onClick={() => setGuildId(guild.id)}
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
                                            <div>no servers...</div>
                                        )}
                                    </ul>
                                )}
                            </Row>
                            <Row className="mb-2">
                                <Form.Group as={Col} controlId="searchID">
                                    <Form.Label>Search Movies</Form.Label>
                                    <InputGroup hasValidation>
                                        <Form.Control
                                            type="search"
                                            aria-describedby="inputGroupPrepend"
                                            name="search"
                                            value={values.search}
                                            onChange={handleChange}
                                            isInvalid={!!errors.search}
                                        />
                                        <Form.Control.Feedback type="invalid">
                                            {errors.search}
                                        </Form.Control.Feedback>
                                    </InputGroup>
                                </Form.Group>
                            </Row>
                            <Button type="submit">Create</Button>
                        </Form>
                    )}
                </Formik>
            </div>
        </div>
    );
}
