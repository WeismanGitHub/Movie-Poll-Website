import { ToastContainer, Toast, Button, Row, Form, Col } from 'react-bootstrap';
import { useNavigate, useSearchParams } from 'react-router-dom';
import DateTimePicker from 'react-datetime-picker';
import { useEffect, useState } from 'react';
import { Field, Formik } from 'formik';
import * as yup from 'yup';
import ky from 'ky';

type Guild = {

}

export default function CreatePoll() {
    const [expirationError, setExpirationError] = useState<string | null>(null);
    const [expiration, setExpiration] = useState<Date | null>(null);

    const [restrictError, setRestrictError] = useState<string | null>(null);
    const [guildId, setGuildId] = useState<string | null>();
    const [code, setCode] = useState<string | null>(null);
    const [restricted, setRestricted] = useState(false);
    const [guilds, setGuilds] = useState<Guild[]>([]);

    // const [error, setError] = useState<object | null>(null);
    const [showError, setShowError] = useState(false);
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    useEffect(() => {
        setCode(searchParams.get('code'));

        if (code) {
            setRestricted(true)
            ky.get(`/api/self/guilds?code=${code}`).json()
            .then((res) => {
                setGuilds(res as Guild[])
            })
            .catch(() => setRestrictError("Could not get your servers."))
        }
    }, []);

    useEffect(() => {
        if (expiration && expiration.valueOf() <= Date.now()) {
            setExpirationError('Must expire in the future.');
        } else {
            setExpirationError(null);
        }
    }, [expiration]);

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
        <div className="container vh-100 text-center align-items-center justify-content-center flex">
            <ToastContainer position="top-end">
                <Toast
                    onClose={() => setShowError(false)}
                    show={showError}
                    autohide={true}
                    className="d-inline-block m-1"
                    bg={'danger'}
                >
                    <Toast.Header>
                        <strong className="me-auto">
                            {/* {error?.message || 'Unable to read error name.'} */}
                        </strong>
                    </Toast.Header>
                    <Toast.Body>
                        {/* {error?.errors &&
                            Object.values(error?.errors).map((err) => {
                                return <div key={err}>{err}</div>;
                            })} */}
                    </Toast.Body>
                </Toast>
            </ToastContainer>
            <div className='center flex-column align-content-center p-2 justify-content-center align-items-center'>
                <div className="row bg-white rounded shadow p-2">
                    <Formik
                        validationSchema={schema}
                        validateOnChange
                        onSubmit={createPoll}
                        initialValues={{ question: '', expirationToggle: false, restrictionToggle: false }}
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
                                        <Form.Label className="me-1">Expiration</Form.Label>
                                        <Field
                                            type="checkbox"
                                            name="expirationToggle"
                                            onChange={() => {
                                                setFieldValue('expirationToggle', !values.expirationToggle);
                                                setExpiration(null);
                                                setExpirationError(null);
                                            }}
                                        />
                                        <Form.Control hidden={true} isInvalid={Boolean(expirationError)} />
                                        <br />
                                        {values.expirationToggle && (
                                            <DateTimePicker
                                                onChange={(value) => setExpiration(value)}
                                                value={expiration}
                                            />
                                        )}
                                        <Form.Control.Feedback type="invalid">
                                            {expirationError}
                                        </Form.Control.Feedback>
                                    </Form.Group>
                                    <Form.Group as={Col}>
                                        <Form.Label className="me-1">Restricted</Form.Label>
                                        <Field
                                            type="checkbox"
                                            name="restrictionToggle"
                                            onChange={() => {
                                                if (!code) {
                                                    localStorage.setItem('redirect', 'create')
                                                    navigate('/auth')
                                                }
                                                
                                                setFieldValue('restrictionToggle', !values.restrictionToggle);
                                            }}
                                        />
                                        <Form.Control hidden={true} isInvalid={Boolean(restrictError)} />
                                        {restricted && <div>
                                            {guilds.length ? guilds.map((guild) => {
                                                console.log(guild)
                                                return <div onClick={() => setGuildId("sdfa")}>test</div>
                                            }) : <div>no servers...</div>}
                                        </div>}
                                        <Form.Control.Feedback type="invalid">
                                            {restrictError}
                                        </Form.Control.Feedback>
                                    </Form.Group>
                                </Row>
                                <Button type="submit">Create</Button>
                            </Form>
                        )}
                    </Formik>
                </div>
            </div>
        </div>
    );
}
