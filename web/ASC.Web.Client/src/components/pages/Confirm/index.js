import React, { useState, useCallback, useEffect } from 'react';
import { withRouter } from "react-router";
import { useTranslation } from 'react-i18next';
import i18n from './i18n';
import { Button, TextInput, PageLayout, Text, PasswordInput } from 'asc-web-components';
import { Container, Row, Col } from 'reactstrap';
import styled from 'styled-components';
import { welcomePageTitle } from './../../../helpers/customNames';

const ConfirmContainer = styled(Container)`
    .confirm-block-title {
        margin: 20px 0px;
    }

    .login-row {
        margin: 23px 0 0;
    }
`;
const mdOptions = { size: 6, offset: 3 };

const passwordSettings = {
    minLength: 6,
    upperCase: true,
    digits: true,
    specSymbols: true
};

const Confirm = (props) => {
    const { t } = useTranslation('translation', { i18n });
    const [email, setEmail] = useState('');
    const [emailValid, setEmailValid] = useState(true);
    const [firstName, setFirstName] = useState('');
    const [firstNameValid, setFirstNameValid] = useState(true);
    const [lastName, setLastName] = useState('');
    const [lastNameValid, setLastNameValid] = useState(true);
    const [password, setPassword] = useState('');
    const [passwordValid, setPasswordValid] = useState(true);
    const [errorText, setErrorText] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const queryString = window.location.search.slice(1);
    const queryParams = queryString.split('&');
    const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
    // const linkParams = Object.fromEntries(arrayOfQueryParams);
    const emailRegex = '[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$';
    const validationEmail = new RegExp(emailRegex);

    const onSubmit = useCallback((e) => {
        //e.preventDefault();

        errorText && setErrorText("");

        let hasError = false;

        if (!validationEmail.test(email.trim())) {
            hasError = true;
            setEmailValid(!hasError);
        }

        if (!firstName.trim()) {
            hasError = true;
            setFirstNameValid(!hasError);
        }

        if (!lastName.trim()) {
            hasError = true;
            setLastNameValid(!hasError);
        }

        if (!password.trim()) {
            hasError = true;
            setPasswordValid(!hasError);
        }

        if (hasError)
            return false;

        setIsLoading(true);


    }, [errorText, email, firstName, lastName, password, validationEmail]);

    const onKeyPress = useCallback((target) => {
        if (target.code === "Enter") {
            onSubmit();
        }
    }, [onSubmit]);


    useEffect(() => {
        window.addEventListener('keydown', onKeyPress);
        window.addEventListener('keyup', onKeyPress);
        // Remove event listeners on cleanup
        return () => {
            window.removeEventListener('keydown', onKeyPress);
            window.removeEventListener('keyup', onKeyPress);
        };
    }, [onKeyPress]);

    return (
        <ConfirmContainer>

            <Row className='confirm-block-title'>
                <Col sm="12" md={mdOptions}>
                    <Text.Body as='p' fontSize={18}>{t('InviteTitle')}</Text.Body>
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <a href='/login'>
                        <img className='login-row' src="images/dark_general.png" alt="Logo" />
                    </a>
                    <Text.Body as='p' fontSize={24} color='#116d9d'>{t('CustomWelcomePageTitle', { welcomePageTitle })}</Text.Body>
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <TextInput
                        type="email"
                        id='email'
                        name='email'
                        value={email}
                        placeholder={t('Email')}
                        size='huge'
                        scale={true}
                        isAutoFocussed={true}
                        tabIndex={1}
                        autoComplete='email'
                        isDisabled={isLoading}
                        hasError={!emailValid}
                        onChange={event => {
                            setEmail(event.target.value);
                            !emailValid && setEmailValid(true);
                            errorText && setErrorText("");
                        }}
                        onKeyDown={event => onKeyPress(event.target)}
                    />
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <TextInput
                        id='name'
                        name='name'
                        value={firstName}
                        placeholder={t('FirstName')}
                        size='huge'
                        scale={true}
                        tabIndex={2}
                        autoComplete='given-name'
                        isDisabled={isLoading}
                        hasError={!firstNameValid}
                        onChange={event => {
                            setFirstName(event.target.value);
                            !firstNameValid && setFirstNameValid(true);
                            errorText && setErrorText("");
                        }}
                        onKeyDown={event => onKeyPress(event.target)}
                    />
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <TextInput
                        id='surname'
                        name='surname'
                        value={lastName}
                        placeholder={t('LastName')}
                        size='huge'
                        scale={true}
                        tabIndex={3}
                        autoComplete='family-name'
                        isDisabled={isLoading}
                        hasError={!lastNameValid}
                        onChange={event => {
                            setLastName(event.target.value);
                            !lastNameValid && setLastNameValid(true);
                            errorText && setErrorText("");
                        }}
                        onKeyDown={event => onKeyPress(event.target)}
                    />
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <PasswordInput
                        inputName="password"
                        emailInputName="email"
                        inputValue={password}
                        placeholder={t('InvitePassword')}
                        size='huge'
                        scale={true}
                        tabIndex={4}
                        hasError={!passwordValid}
                        onChange={event => {
                            setPassword(event.target.value);
                            !passwordValid && setPasswordValid(true);
                            errorText && setErrorText("");
                            onKeyPress(event.target);
                        }}
                        clipActionResource={t('CopyEmailAndPassword')}
                        clipEmailResource={`${t('Email')}: `}
                        clipPasswordResource={`${t('InvitePassword')}: `}
                        tooltipPasswordTitle={`${t('ErrorPasswordMessage')}:`}
                        tooltipPasswordLength={`${t('ErrorPasswordLength', { fromNumber: 6, toNumber:30 })}:`}
                        tooltipPasswordDigits={t('ErrorPasswordNoDigits')}
                        tooltipPasswordCapital={t('ErrorPasswordNoUpperCase')}
                        tooltipPasswordSpecial={`${t('ErrorPasswordNoSpecialSymbols')} (!@#$%^&*)`}
                        generatorSpecial="!@#$%^&*"
                        passwordSettings={passwordSettings}
                        isDisabled={isLoading}
                    />
                </Col>
            </Row>

            <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <Button
                        primary
                        size='big'
                        label={t('LoginRegistryButton')}
                        tabIndex={5}
                        isDisabled={isLoading}
                        isLoading={isLoading}
                        onClick={onSubmit}
                    />
                </Col>
            </Row>

            {/*             <Row className='login-row'>
                <Col sm="12" md={mdOptions}>
                    <Text.Body as='p' fontSize={14}>{t('LoginWithAccount')}</Text.Body>
                </Col>
            </Row>
 */}
        </ConfirmContainer>
    );
}

const ConfirmForm = (props) => (<PageLayout sectionBodyContent={<Confirm {...props} />} />);

export default withRouter(ConfirmForm);