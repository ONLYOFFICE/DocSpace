import React, { useState, useCallback } from 'react';
import { Collapse, Container, Row, Col } from 'reactstrap';
import { storiesOf } from '@storybook/react';
import TextInput from '../text-input';
import Button from '../button';

const LoginForm = props => {
    const { loginPlaceholder, passwordPlaceholder, buttonText, onSubmit, errorText } = props;
    const [login, setLogin] = useState('');
    const [loginValid, setLoginValid] = useState(true);
    const [password, setPassword] = useState('');
    const [passwordValid, setPasswordValid] = useState(true);

    const validateAndSubmit = useCallback((event) => {
        if (!login.trim())
            setLoginValid(false);

        if (!password.trim())
            setPasswordValid(false);

        if (loginValid && passwordValid)
            return onSubmit(event, { login, password });

        return false;
    }, []);

    const onLoginChange = useCallback(event => {
        setLogin(event.target.value);
        setLoginValid(true);
    }, [])

    const onPasswordChange = useCallback(event => {
        setPassword(event.target.value);
        setPasswordValid(true);
    }, []);

    return (
        <Container>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <TextInput
                        id="login"
                        name="login"
                        hasError={!loginValid}
                        value={login}
                        placeholder={loginPlaceholder}
                        size='huge'
                        scale={true}
                        isAutoFocussed={true}
                        tabIndex={1}
                        onChange={onLoginChange} />
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
                        placeholder={passwordPlaceholder}
                        size='huge'
                        scale={true}
                        tabIndex={2}
                        onChange={onPasswordChange} 
                    />
                </Col>
            </Row>
            <Row style={{ margin: "23px 0 0" }}>
                <Col sm="12" md={{ size: 6, offset: 3 }}>
                    <Button primary size='big' tabIndex={3} onClick={validateAndSubmit} label={buttonText} />
                </Col>
            </Row>
            <Collapse isOpen={errorText}>
                <Row>
                    <Col>
                        <span>{errorText}</span>
                    </Col>
                </Row>
            </Collapse>
        </Container>
    );
};

storiesOf('EXAMPLES|Forms', module)
    // To set a default viewport for all the stories for this component
    .addParameters({ viewport: { defaultViewport: 'responsive' } })
    .add('login', () => {
        const onSubmit = (e, credentials) => {
            console.log("onSubmit", e, credentials);
        };
        return (
            <LoginForm
                loginPlaceholder='You registration email'
                passwordPlaceholder='Password'
                buttonText='Sign In'
                onSubmit={onSubmit} />
        )
    });