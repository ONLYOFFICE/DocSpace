import React from 'react';
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';
import { Button, TextInput, Text, PasswordInput, toastr, Loader } from 'asc-web-components';
import { PageLayout } from "asc-web-common";
import styled from 'styled-components';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { constants } from 'asc-web-common';
import { getConfirmationInfo, activateConfirmUser } from '../../../../store/confirm/actions';
const { EmployeeActivationStatus } = constants;


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

    .display-none {
      display: none;
    }

`;

const emailInputName = 'email';

class Confirm extends React.PureComponent {

  constructor(props) {
    super(props);

    this.state = {
      email: props.linkData.email,
      firstName: props.linkData.firstname,
      firstNameValid: true,
      lastName: props.linkData.lastname,
      lastNameValid: true,
      password: '',
      passwordValid: true,
      errorText: '',
      isLoading: false,
      passwordEmpty: false,
      key: props.linkData.confirmHeader,
      linkType: props.linkData.type,
      userId: props.linkData.uid
    };
  }

  onSubmit = (e) => {
    this.setState({ isLoading: true }, function () {
      const { activateConfirmUser, history } = this.props;

      this.setState({ errorText: "" });

      let hasError = false;

      if (!this.state.firstName.trim()) {
        hasError = true;
        this.setState({ firstNameValid: !hasError });
      }

      if (!this.state.lastName.trim()) {
        hasError = true;
        this.setState({ lastNameValid: !hasError });
      }

      if (!this.state.passwordValid) {
        hasError = true;
        this.setState({ passwordValid: !hasError });
      }

      if (!this.state.password.trim()) {
        this.setState({ passwordEmpty: true });
        hasError = true;
      }

      if (hasError) {
        this.setState({ isLoading: false });
        return false;
      }

      const loginData = {
        userName: this.state.email,
        password: this.state.password
      };

      const personalData = {
        firstname: this.state.firstName,
        lastname: this.state.lastName
      };
      activateConfirmUser(personalData, loginData, this.state.key, this.state.userId, EmployeeActivationStatus.Activated)
        .then(() => history.push('/'))
        .catch(error => {
          console.error("activate error", error);
          this.setState({
            errorText: error,
            isLoading: false
          });
        });
    });
  };

  onKeyPress = (event) => {
    if (event.key === "Enter") {
      this.onSubmit();
    }
  };

  onCopyToClipboard = () => toastr.success(this.props.t('EmailAndPasswordCopiedToClipboard'));
  validatePassword = (value) => this.setState({ passwordValid: value });

  componentDidMount() {
    const { getConfirmationInfo, history } = this.props;

    getConfirmationInfo(this.state.key)
      .then(
        function () {
          console.log("get settings success");
        }
      )
      .catch(e => {
        console.error("get settings error", e);
        history.push(`/login/error=${e}`);
      });

    window.addEventListener('keydown', this.onKeyPress);
    window.addEventListener('keyup', this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener('keydown', this.onKeyPress);
    window.removeEventListener('keyup', this.onKeyPress);
  }

  onChangeName = event => {
    this.setState({ firstName: event.target.value });
    !this.state.firstNameValid && this.setState({ firstNameValid: event.target.value });
    this.state.errorText && this.setState({ errorText: "" });
  }

  onChangeSurname = event => {
    this.setState({ lastName: event.target.value });
    !this.state.lastNameValid && this.setState({ lastNameValid: true });
    this.state.errorText && this.setState({ errorText: "" });;
  }

  onChangePassword = event => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    (event.target.value.trim()) && this.setState({ passwordEmpty: false });
    this.state.errorText && this.setState({ errorText: "" });
    this.onKeyPress(event);
  }

  render() {
    console.log('ActivateUser render');
    const { settings, isConfirmLoaded, t, greetingTitle } = this.props;
    return (
      !isConfirmLoaded
        ? (
          <Loader className="pageLoader" type="rombs" size='40px' />
        )
        : (
          <ConfirmContainer>
            <div className='start-basis'>
              <div className='margin-left'>
                <Text className='confirm-row' as='p' fontSize='18px'>{t('InviteTitle')}</Text>

                <div className='confirm-row full-width break-word'>
                  <a href='/login'>
                    <img src="images/dark_general.png" alt="Logo" />
                  </a>
                  <Text as='p' fontSize='24px' color='#116d9d'>{greetingTitle}</Text>
                </div>
              </div>

              <div>
                <div className='full-width'>

                  <TextInput
                    className='confirm-row'
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
                    onChange={this.onChangeName}
                    onKeyDown={this.onKeyPress}
                  />

                  <TextInput
                    className='confirm-row'
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
                    onChange={this.onChangeSurname}
                    onKeyDown={this.onKeyPress}
                  />

                  <TextInput
                    className='confirm-row display-none'
                    id='email'
                    name={emailInputName}
                    value={this.state.email}
                    size='huge'
                    scale={true}
                    isReadOnly={true}
                  />

                </div>

                <PasswordInput
                  className='confirm-row'
                  id='password'
                  inputName='password'
                  emailInputName={emailInputName}
                  inputValue={this.state.password}
                  placeholder={t('InvitePassword')}
                  size='huge'
                  scale={true}
                  tabIndex={4}
                  maxLength={30}
                  inputWidth={inputWidth}
                  hasError={this.state.passwordEmpty}
                  onChange={this.onChangePassword}
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
                  onKeyDown={this.onKeyPress}
                />


                <Button
                  className='confirm-row'
                  primary
                  size='big'
                  label={t('LoginRegistryButton')}
                  tabIndex={5}
                  isLoading={this.state.isLoading}
                  onClick={this.onSubmit}
                />

              </div>

              {/*             <Row className='confirm-row'>

                    <Text as='p' fontSize='14px'>{t('LoginWithAccount')}</Text>

            </Row>
 */}
              <Text className="confirm-row" fontSize='14px' color="#c30">
                {this.state.errorText}
              </Text>
            </div>
          </ConfirmContainer>
        )
    );
  }
}


Confirm.propTypes = {
  getConfirmationInfo: PropTypes.func.isRequired,
  activateConfirmUser: PropTypes.func.isRequired,
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired
};
const ActivateUserForm = (props) => (<PageLayout sectionBodyContent={<Confirm {...props} />} />);


function mapStateToProps(state) {
  return {
    isConfirmLoaded: state.confirm.isConfirmLoaded,
    settings: state.auth.settings.passwordSettings,
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(mapStateToProps, { getConfirmationInfo, activateConfirmUser })(withRouter(withTranslation()(ActivateUserForm)));