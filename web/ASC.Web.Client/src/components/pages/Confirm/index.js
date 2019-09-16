import React from 'react';
import { withRouter } from "react-router";
import { I18nextProvider, withTranslation } from 'react-i18next';
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

const emailRegex = '[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$';
const validationEmail = new RegExp(emailRegex);

class Confirm extends React.PureComponent {

    constructor(props) {
        super(props);

        this.state = {
            email: '',
            emailValid: true,
            firstName: '',
            firstNameValid: true,
            lastName: '',
            lastNameValid: true,
            password: '',
            passwordValid: true,
            errorText: '',
            isLoading: false,
        };
    }

    onSubmit = (e) => {

        const { location, history, createConfirmUser } = this.props;
        const queryString = location.search.slice(1);
        const queryParams = queryString.split('&');
        const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
        const linkParams = Object.fromEntries(arrayOfQueryParams);
        const isVisitor = parseInt(linkParams.emplType) === 2;

        this.state.errorText && this.setState({ errorText: "" });

        let hasError = false;

        if (!this.state.firstName.trim()) {
            hasError = true;
            this.setState({ firstNameValid: !hasError });
        }

        if (!this.state.lastName.trim()) {
            hasError = true;
            this.setState({ lastNameValid: !hasError });
        }

        if (!validationEmail.test(this.state.email.trim())) {
            hasError = true;
            this.setState({ emailValid: !hasError });
        }

        if (!this.state.passwordValid) {
            hasError = true;
            this.setState({ passwordValid: !hasError });
        }

        if (hasError)
            return false;

        this.setState({ isLoading: true });

        const loginData = {
            userName: this.state.email,
            password: this.state.password
        }
        const registerData = {
            firstname: this.state.firstName,
            lastname: this.state.lastName,
            email: this.state.email,
            isVisitor: isVisitor
        };
        createConfirmUser(registerData, loginData, queryString)
            .then(() => history.push('/'))
            .catch(e => {
                console.error("confirm error", e);
                this.setState({ errorText: e.message });
                this.setState({ isLoading: false });
            });
    };

    onKeyPress = (target) => {
        if (target.code === "Enter") {
            this.onSubmit();
        }
    };

    onCopyToClipboard = () => toastr.success(this.props.t('EmailAndPasswordCopiedToClipboard'));
    validatePassword = (value) => this.setState({ passwordValid: value });

    componentDidMount() {
        const { getPasswordSettings, history, location } = this.props;
        const queryString = location.search.slice(1);

        getPasswordSettings(queryString)
            .then(
                function () {
                    console.log("get settings success");
                }
            )
            .catch(e => {
                console.error("get settings error", e);
                history.push(`/login/${e}`);
            });

        window.addEventListener('keydown', this.onKeyPress);
        window.addEventListener('keyup', this.onKeyPress);
    }

    componentWillUnmount() {
        window.removeEventListener('keydown', this.onKeyPress);
        window.removeEventListener('keyup', this.onKeyPress);
    }

    render() {
        const { settings, isLoaded, t } = this.props;
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
                                            value={this.state.firstName}
                                            placeholder={t('FirstName')}
                                            size='huge'
                                            scale={true}
                                            tabIndex={1}
                                            isAutoFocussed={true}
                                            autoComplete='given-name'
                                            isDisabled={this.state.isLoading}
                                            hasError={!this.state.firstNameValid}
                                            onChange={event => {
                                                this.setState({ firstName: event.target.value });
                                                !this.state.firstNameValid && this.setState({ firstNameValid: event.target.value });
                                                this.state.errorText && this.setState({ errorText: "" });
                                            }}
                                            onKeyDown={event => this.onKeyPress(event.target)}
                                        />
                                    </FieldContainer>

                                    <FieldContainer isVertical={true} className=''>
                                        <TextInput
                                            id='surname'
                                            name='surname'
                                            value={this.state.lastName}
                                            placeholder={t('LastName')}
                                            size='huge'
                                            scale={true}
                                            tabIndex={2}
                                            autoComplete='family-name'
                                            isDisabled={this.state.isLoading}
                                            hasError={!this.state.lastNameValid}
                                            onChange={event => {
                                                this.setState({ lastName: event.target.value });
                                                !this.state.lastNameValid && this.setState({ lastNameValid: true });
                                                this.state.errorText && this.setState({ errorText: "" });;
                                            }}
                                            onKeyDown={event => this.onKeyPress(event.target)}
                                        />
                                    </FieldContainer>

                                    <FieldContainer isVertical={true} className=''>
                                        <TextInput
                                            id='email'
                                            name={emailInputName}
                                            value={this.state.email}
                                            placeholder={t('Email')}
                                            size='huge'
                                            scale={true}
                                            tabIndex={3}
                                            autoComplete='email'
                                            isDisabled={this.state.isLoading}
                                            hasError={!this.state.emailValid}
                                            onChange={event => {
                                                this.setState({ email: event.target.value });
                                                !this.state.emailValid && this.setState({ emailValid: true });
                                                this.state.errorText && this.setState({ errorText: "" });;
                                            }}
                                            onKeyDown={event => this.onKeyPress(event.target)}
                                        />

                                    </FieldContainer>
                                </div>

                                <FieldContainer isVertical={true} className=''>
                                    <PasswordInput
                                        inputName={passwordInputName}
                                        emailInputName={emailInputName}
                                        inputValue={this.state.password}
                                        placeholder={t('InvitePassword')}
                                        size='huge'
                                        scale={true}
                                        tabIndex={4}
                                        maxLength={30}
                                        inputWidth={inputWidth}
                                        hasError={!this.state.passwordValid && !this.state.password.trim()}
                                        onChange={event => {
                                            this.setState({ password: event.target.value });
                                            !this.state.passwordValid && this.setState({ passwordValid: true });
                                            this.state.errorText && this.setState({ errorText: "" });
                                            this.onKeyPress(event.target);
                                        }}
                                        onCopyToClipboard={this.onCopyToClipboard}
                                        onValidateInput={this.validatePassword}
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
                                        isDisabled={this.state.isLoading}
                                    />
                                </FieldContainer>

                                <Button
                                    primary
                                    size='big'
                                    label={t('LoginRegistryButton')}
                                    tabIndex={5}
                                    isDisabled={this.state.isLoading}
                                    isLoading={this.state.isLoading}
                                    onClick={this.onSubmit}
                                />

                            </div>

                            {/*             <Row className='confirm-row'>

                    <Text.Body as='p' fontSize={14}>{t('LoginWithAccount')}</Text.Body>

            </Row>
 */}
                            <Collapse className='confirm-row'
                                isOpen={!!this.state.errorText}>
                                <div className="alert alert-danger">{this.state.errorText}</div>
                            </Collapse>
                        </div>
                    </ConfirmContainer>
                )
        );
    }
}

const ConfirmWrapper = withTranslation()(Confirm);

const ConfirmWithTrans = (props) => <I18nextProvider i18n={i18n}><ConfirmWrapper {...props} /></I18nextProvider>;

ConfirmWithTrans.propTypes = {
};
const ConfirmForm = (props) => (<PageLayout sectionBodyContent={<ConfirmWithTrans {...props} />} />);


function mapStateToProps(state) {
    return {
        isLoaded: state.auth.isLoaded,
        settings: state.auth.password
    };
}

export default connect(mapStateToProps, { getPasswordSettings, createConfirmUser })(withRouter(ConfirmForm));