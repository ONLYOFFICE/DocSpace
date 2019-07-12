import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Collapse, Container, Row, Col, Card, CardTitle, CardImg } from 'reactstrap';
import { Button, TextInput, PageLayout } from 'asc-web-components';
import { connect } from 'react-redux';
import { login } from '../../../actions/authActions';

const Form = props => {
    const [identifier, setIdentifier] = useState('');
    const [identifierValid, setIdentifierValid] = useState(true);
    const [password, setPassword] = useState('');
    const [passwordValid, setPasswordValid] = useState(true);
    const [errorText, setErrorText] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const { login, match, location, history } = props;

    const onSubmit = (e) => {
        e.preventDefault();

        errorText && setErrorText("");

        let hasError = false;

        if (!identifier.trim()) {
            hasError = true;
            setIdentifierValid(!hasError);
        }

        if (!password.trim()) {
            hasError = true;
            setPasswordValid(!hasError);
        }

        if (hasError)
            return false;

        setIsLoading(true);

        let payload = {
            userName: identifier,
            password: password
        };

        login(payload)
            .then(function () {
                console.log("auth success", match, location, history);
                setIsLoading(false)
                history.push('/');
            })
            .catch(e => {
                console.error("auth error", e);
                setErrorText(e.message);
                setIsLoading(false)
            });
    };

    return (
        <Container style={{ marginTop: "70px" }}>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <Card style={{ border: 'none' }}>
                        <CardImg top style={{ maxWidth: '216px', maxHeight: '35px' }} src="images/dark_general.png" alt="Logo" />
                        <CardTitle style={{ wordWrap: 'break-word', margin: '8px 0', textAlign: 'left', fontSize: '24px', color: '#116d9d' }}>Cloud Office Applications</CardTitle>
                    </Card>
                </Col>
            </Row>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <TextInput
                        id="login"
                        name="login"
                        hasError={!identifierValid}
                        value={identifier}
                        placeholder="You registration email"
                        size='huge'
                        scale={true}
                        isAutoFocussed={true}
                        tabIndex={1}
                        isDisabled={isLoading}
                        autocomple="username"
                        onChange={event => {
                            setIdentifier(event.target.value);
                            !identifierValid && setIdentifierValid(true);
                            errorText && setErrorText("");
                        }} />
                </Col>
            </Row>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <TextInput
                        id="password"
                        name="password"
                        type="password"
                        hasError={!passwordValid}
                        value={password}
                        placeholder="Password"
                        size='huge'
                        scale={true}
                        tabIndex={2}
                        isDisabled={isLoading}
                        autocomple="current-password"
                        onChange={event => {
                            setPassword(event.target.value);
                            !passwordValid && setPasswordValid(true);
                            errorText && setErrorText("");
                        }} />
                </Col>
            </Row>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <Button
                        primary
                        size='big'
                        label={isLoading ? "Loading..." : "Sign In"}
                        tabIndex={3}
                        isDisabled={isLoading}
                        isLoading={isLoading}
                        onClick={onSubmit} />
                </Col>
            </Row>
            <Collapse isOpen={ !!errorText }>
                <Row style={{ margin: "23px 0 0" }}>
                    <Col sm="12" md={{ size: 6, offset: 3 }}>
                        <div className="alert alert-danger">{errorText}</div>
                    </Col>
                </Row>
            </Collapse>
        </Container>
    );
}

const LoginForm = (props) => <PageLayout sectionBodyContent={<Form {...props} />} />;

LoginForm.propTypes = {
    login: PropTypes.func.isRequired,
    match: PropTypes.object.isRequired,
    location: PropTypes.object.isRequired,
    history: PropTypes.object.isRequired
}

LoginForm.defaultProps = {
    identifier: "",
    password: ""
}

export default connect(null, { login })(withRouter(LoginForm));