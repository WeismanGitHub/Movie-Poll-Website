import { ToastContainer, Toast, Button, Row, Form, Col } from 'react-bootstrap';
import DateTimePicker from 'react-datetime-picker';
import { useSearchParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { Field, Formik } from 'formik';
import * as yup from 'yup';

export default function CreatePoll() {
    const [expirationError, setExpirationError] = useState<string | null>(null);
    const [expiration, setExpiration] = useState<Date | null>(null);
    const [error, setError] = useState<object | null>(null);
    const [showError, setShowError] = useState(false);
    const [searchParams] = useSearchParams();

    useEffect(() => {
        const pollId = searchParams.get('pollId');
        const state = searchParams.get('state');
        const code = searchParams.get('code');

        console.log(pollId, state, code);
        error;
        setError;
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
        console.log(values);
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

            <div className="row bg-white rounded shadow p-2">
                <Formik
                    validationSchema={schema}
                    validateOnChange
                    onSubmit={createPoll}
                    initialValues={{ question: '', expirationToggle: false }}
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
                            </Row>
                            <Button type="submit">Create</Button>
                        </Form>
                    )}
                </Formik>
            </div>
        </div>
    );
}
