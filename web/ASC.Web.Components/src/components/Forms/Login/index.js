import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Collapse, Container, Row, Col } from 'reactstrap';
import styled from 'styled-components';
import Button from '../../button';
import TextInput from '../../text-input';

const FormRow = styled(Row)`
    margin: 23px 0 0;
`

const LoginForm = props => {
    const {loginPlaceholder, passwordPlaceholder, buttonText, onSubmit, errorText} = props;
    const [login, setLogin] = useState('');
    const [loginValid, setLoginValid] = useState(true);
    const [password, setPassword] = useState('');
    const [passwordValid, setPasswordValid] = useState(true);

    const validateAndSubmit = (event) => {
        if (!login.trim())
            setLoginValid(false);

        if (!password.trim())
            setPasswordValid(false);

        if (loginValid && passwordValid)
            return onSubmit(event, { login, password });

        return false;
    };

    return (
        <Container>
            <FormRow>
                <Col>
                    <TextInput
                        id="login"
                        name="login"
                        hasError={!loginValid}
                        value={login}
                        placeholder={loginPlaceholder}
                        size='big'
                        isAutoFocussed={true}
                        tabIndex={1}
                        onChange={event => { 
                            setLogin(event.target.value);
                            setLoginValid(true);
                        }} />
                </Col>
            </FormRow>
            <FormRow>
                <Col>
                    <TextInput
                        id="password"
                        name="password"
                        type="password"
                        hasError={!passwordValid}
                        value={password}
                        placeholder={passwordPlaceholder}
                        size='big'
                        tabIndex={2}
                        onChange={event => { 
                            setPassword(event.target.value);
                            setPasswordValid(true);
                        }} />
                </Col>
            </FormRow>
            <FormRow>
                <Col>
                    <Button primary size='big' tabIndex={3} onClick={validateAndSubmit}>{buttonText}</Button>
                </Col>
            </FormRow>
            <Collapse isOpen={errorText}>
                <Row >
                    <Col>
                        <span>{errorText}</span>
                    </Col>
                </Row>
            </Collapse>
        </Container>
    );
}

LoginForm.propTypes = {
    loginPlaceholder: PropTypes.string.isRequired,
    passwordPlaceholder: PropTypes.string.isRequired,
    buttonText: PropTypes.string.isRequired,
    errorText: PropTypes.string,
    
    onSubmit: PropTypes.func.isRequired
}

LoginForm.defaultProps = {
    login: '',
    password: ''
}

export default LoginForm
