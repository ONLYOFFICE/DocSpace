import React, { useState, useCallback, useEffect } from 'react';
import { withRouter } from "react-router";
import { useTranslation } from 'react-i18next';
import i18n from './i18n';
import { Button, TextInput, PageLayout, Text, PasswordInput, FieldContainer, toastr, Loader } from 'asc-web-components';
import styled from 'styled-components';
import { welcomePageTitle } from './../../../helpers/customNames';
import { Collapse } from 'reactstrap';
import { connect } from 'react-redux';
import { getPasswordSettings, createConfirmUser } from '../../../store/auth/actions';

const inputWidth = '400px';

const ConfirmContainer = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-left: 200px;

    @media (max-width: 830px) {
            margin-left: 40px;
        }

    .start-basis {
        align-items: flex-start;
    }
    
    .margin-left {
        margin-left: 20px;
    }

    .full-width {
        width: ${inputWidth}
    }

    .confirm-row {
        margin: 23px 0 0;
    }

    .break-word {
        word-break: break-word;
    }

`;


const emailInputName = 'email';
const passwordInputName = 'password';

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
    const { location, history, isLoaded, getPasswordSettings, settings, createConfirmUser } = props;

    const queryString = location.search.slice(1);
    const queryParams = queryString.split('&');
    const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
    const linkParams = Object.fromEntries(arrayOfQueryParams);
    const isVisitor = parseInt(linkParams.emplType) === 2;

    const emailRegex = '[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z0-9]{2,}$';
    const validationEmail = new RegExp(emailRegex);
    const onSubmit = useCallback((e) => {

        errorText && setErrorText("");

        let hasError = false;

        if (!firstName.trim()) {
            hasError = true;
            setFirstNameValid(!hasError);
        }

        if (!lastName.trim()) {
            hasError = true;
            setLastNameValid(!hasError);
        }

        if (!validationEmail.test(email.trim())) {
            hasError = true;
            setEmailValid(!hasError);
        }

        if (!passwordValid) {
            hasError = true;
            setPasswordValid(!hasError);
        }

        if (hasError)
            return false;

        setIsLoading(true);

        const loginData = {
            userName: email,
            password: password
        }
        const registerData = {
            firstname: firstName,
            lastname: lastName,
            email: email,
            isVisitor: isVisitor
        };

        createConfirmUser(registerData, loginData, queryString)
            .then(() => history.push('/'))
            .catch(e => {
                console.error("confirm error", e);
                setErrorText(e.message);
                setIsLoading(false)
            });

    }, [errorText, email, firstName, lastName, validationEmail, passwordValid, createConfirmUser, history, queryString, isVisitor, password]);

    const onKeyPress = useCallback((target) => {
        if (target.code === "Enter") {
            onSubmit();
        }
    }, [onSubmit]);


    useEffect(() => {
        if (!isLoaded && !settings) {
            getPasswordSettings(queryString)
                .then(
                    function () {
                        console.log("get settings success");
                    }
                )
                .catch(e => {
                    console.error("get settings error", e);
                    history.push(`/login/${e}`);

                })
        };

        window.addEventListener('keydown', onKeyPress);
        window.addEventListener('keyup', onKeyPress);

        // Remove event listeners on cleanup
        return () => {
            window.removeEventListener('keydown', onKeyPress);
            window.removeEventListener('keyup', onKeyPress);
        };
    }, [onKeyPress, getPasswordSettings, isLoaded, settings, queryString, history]);

    const onCopyToClipboard = () => toastr.success(t('EmailAndPasswordCopiedToClipboard'));
    const validatePassword = (value) => setPasswordValid(value);

    return (
        !isLoaded
            ? (
                <Loader className="pageLoader" type="rombs" size={40} />
            )
            : (
                <ConfirmContainer>
                    <div className='start-basis'>
                        <div className='margin-left'>
                            <Text.Body className='confirm-row' as='p' fontSize={18}>{t('InviteTitle')}</Text.Body>

                            <div className='confirm-row full-width break-word'>
                                <a href='/login'>
                                    <img src="images/dark_general.png" alt="Logo" />
                                </a>
                                <Text.Body as='p' fontSize={24} color='#116d9d'>{t('CustomWelcomePageTitle', { welcomePageTitle })}</Text.Body>
                            </div>
                        </div>

                        <div className='confirm-row'>
                            <div className='full-width'>
                                <FieldContainer isVertical={true} className=''>
                                    <TextInput
                                        id='name'
                                        name='name'
                                        value={firstName}
                                        placeholder={t('FirstName')}
                                        size='huge'
                                        scale={true}
                                        tabIndex={1}
                                        isAutoFocussed={true}
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

                                </FieldContainer>

                                <FieldContainer isVertical={true} className=''>

                                    <TextInput
                                        id='surname'
                                        name='surname'
                                        value={lastName}
                                        placeholder={t('LastName')}
                                        size='huge'
                                        scale={true}
                                        tabIndex={2}
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

                                </FieldContainer>

                                <FieldContainer isVertical={true} className=''>
                                    <TextInput
                                        id='email'
                                        name={emailInputName}
                                        value={email}
                                        placeholder={t('Email')}
                                        size='huge'
                                        scale={true}
                                        tabIndex={3}
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

                                </FieldContainer>
                            </div>

                            <FieldContainer isVertical={true} className=''>
                                <PasswordInput
                                    inputName={passwordInputName}
                                    emailInputName={emailInputName}
                                    inputValue={password}
                                    placeholder={t('InvitePassword')}
                                    size='huge'
                                    scale={true}
                                    tabIndex={4}
                                    maxLength={30}
                                    inputWidth={inputWidth}
                                    hasError={!passwordValid && !password.trim()}
                                    onChange={event => {
                                        setPassword(event.target.value);
                                        !passwordValid && setPasswordValid(true);
                                        errorText && setErrorText("");
                                        onKeyPress(event.target);
                                    }}
                                    onCopyToClipboard={onCopyToClipboard}
                                    onValidateInput={validatePassword}
                                    clipActionResource={t('CopyEmailAndPassword')}
                                    clipEmailResource={`${t('Email')}: `}
                                    clipPasswordResource={`${t('InvitePassword')}: `}
                                    tooltipPasswordTitle={`${t('ErrorPasswordMessage')}:`}
                                    tooltipPasswordLength={`${t('ErrorPasswordLength', { fromNumber: 6, toNumber: 30 })}:`}
                                    tooltipPasswordDigits={t('ErrorPasswordNoDigits')}
                                    tooltipPasswordCapital={t('ErrorPasswordNoUpperCase')}
                                    tooltipPasswordSpecial={`${t('ErrorPasswordNoSpecialSymbols')} (!@#$%^&*)`}
                                    generatorSpecial="!@#$%^&*"
                                    passwordSettings={settings}
                                    isDisabled={isLoading}
                                />
                            </FieldContainer>

                            <Button
                                primary
                                size='big'
                                label={t('LoginRegistryButton')}
                                tabIndex={5}
                                isDisabled={isLoading}
                                isLoading={isLoading}
                                onClick={onSubmit}
                            />

                        </div>

                        {/*             <Row className='confirm-row'>

                    <Text.Body as='p' fontSize={14}>{t('LoginWithAccount')}</Text.Body>

            </Row>
 */}
                        <Collapse className='confirm-row'
                            isOpen={!!errorText}>
                            <div className="alert alert-danger">{errorText}</div>
                        </Collapse>
                    </div>
                </ConfirmContainer>
            )
    );
}

const ConfirmForm = (props) => (<PageLayout sectionBodyContent={<Confirm {...props} />} />);

function mapStateToProps(state) {
    return {
        isLoaded: state.auth.isLoaded,
        settings: state.auth.password
    };
}

export default connect(mapStateToProps, { getPasswordSettings, createConfirmUser })(withRouter(ConfirmForm));