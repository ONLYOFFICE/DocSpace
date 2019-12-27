import React, { Component } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  Button,
  TextInput,
  Text,
  Heading,
  Link,
  toastr,
  Checkbox,
  HelpButton
} from "asc-web-components";
import PageLayout from "../../components/PageLayout";
import { connect } from "react-redux";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import SubModalDialog from "./sub-components/modal-dialog";
import { login, setIsLoaded } from "../../store/auth/actions";
import { sendInstructionsToChangePassword } from "../../api/people";

const FormContainer = styled.form`
  margin: 50px auto 0 auto;
  max-width: 432px;

  .login-header {
    min-height: 79px;
    margin-bottom: 24px;

    .login-logo {
      max-width: 216px;
      max-height: 35px;
    }

    .login-title {
      margin: 8px 0;
    }
  }

  .login-input {
    margin-bottom: 24px;
  }

  .login-forgot-wrapper {
    height: 36px;

    .login-checkbox-wrapper {
      position: absolute;
      display: inline-flex;

      .login-checkbox {
        float: left;
        span {
          font-size: 12px;
        }
      }
      .login-tooltip {
        display: inline-flex;
        
        @media(min-width: 1025px) {
          margin-left: 8px;
          margin-top: 4px;
        }
        @media(max-width: 1024px) {
          padding: 4px 8px 8px 8px;
        }        
      }
    }
    .login-link {
      float: right;
      line-height: 16px;
    }
  }

  .login-button {
    margin-bottom: 16px;
  }

  .login-button-dialog {
    margin-right: 8px;
  }
`;

class Form extends Component {
  constructor(props) {
    super(props);

    this.state = {
      identifierValid: true,
      identifier: "",
      isLoading: false,
      isDisabled: false,
      passwordValid: true,
      password: "",
      isChecked: false,
      openDialog: false,
      email: "",
      errorText: ""
    };
  }

  onChangeLogin = event => {
    this.setState({ identifier: event.target.value });
    !this.state.identifierValid && this.setState({ identifierValid: true });
    this.state.errorText && this.setState({ errorText: "" });
  };

  onChangePassword = event => {
    this.setState({ password: event.target.value });
    !this.state.passwordValid && this.setState({ passwordValid: true });
    this.state.errorText && this.setState({ errorText: "" });
  };

  onChangeEmail = event => {
    this.setState({ email: event.target.value });
  };

  onChangeCheckbox = () => this.setState({ isChecked: !this.state.isChecked });

  onClick = () => {
    this.setState({
      openDialog: true,
      isDisabled: true,
      email: this.state.identifier
    });
  };

  onKeyPress = event => {
    if (event.key === "Enter") {
      !this.state.isDisabled
        ? this.onSubmit()
        : this.onSendPasswordInstructions();
    }
  };

  onSendPasswordInstructions = () => {
    this.setState({ isLoading: true });
    sendInstructionsToChangePassword(this.state.email)
      .then(
        res => toastr.success(res),
        message => toastr.error(message)
      )
      .finally(this.onDialogClose());
  };

  onDialogClose = () => {
    this.setState({
      openDialog: false,
      isDisabled: false,
      isLoading: false,
      email: ""
    });
  };

  onSubmit = () => {
    const { errorText, identifier, password } = this.state;
    const { login, setIsLoaded, history } = this.props;

    errorText && this.setState({ errorText: "" });
    let hasError = false;

    const userName = identifier.trim();

    if (!userName) {
      hasError = true;
      this.setState({ identifierValid: !hasError });
    }

    const pass = password.trim();

    if (!pass) {
      hasError = true;
      this.setState({ passwordValid: !hasError });
    }

    if (hasError) return false;

    this.setState({ isLoading: true });

    login(userName, pass)
      .then(() => {
        setIsLoaded(true);
        history.push("/");
      })
      .catch(error => {
        this.setState({ errorText: error, isLoading: false });
      });
  };

  componentDidMount() {
    const { language, match, i18n } = this.props;
    const { error, confirmedEmail } = match.params;

    if(i18n.lng != language)
      i18n.changeLanguage(language);

    error && this.setState({ errorText: error });
    confirmedEmail && this.setState({ identifier: confirmedEmail });
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
  }

  render() {
    const { greetingTitle, match, t } = this.props;

    const {
      identifierValid,
      identifier,
      isLoading,
      passwordValid,
      password,
      isChecked,
      openDialog,
      email,
      errorText
    } = this.state;
    const { params } = match;

    //console.log("Login render");

    return (
      <FormContainer>
        <div className="login-header">
          <img
            className="login-logo"
            src="images/dark_general.png"
            alt="Logo"
          />
          <Heading className="login-title" color="#116d9d">
            {greetingTitle}
          </Heading>
        </div>

        <TextInput
          id="login"
          name="login"
          hasError={!identifierValid}
          value={identifier}
          placeholder={t("RegistrationEmailWatermark")}
          size="huge"
          scale={true}
          isAutoFocussed={true}
          tabIndex={1}
          isDisabled={isLoading}
          autoComplete="username"
          onChange={this.onChangeLogin}
          onKeyDown={this.onKeyPress}
          className="login-input"
        />

        <TextInput
          id="password"
          name="password"
          type="password"
          hasError={!passwordValid}
          value={password}
          placeholder={t("Password")}
          size="huge"
          scale={true}
          tabIndex={2}
          isDisabled={isLoading}
          autoComplete="current-password"
          onChange={this.onChangePassword}
          onKeyDown={this.onKeyPress}
          className="login-input"
        />

        <div className="login-forgot-wrapper">
          <div className="login-checkbox-wrapper">
            <Checkbox
              className="login-checkbox"
              isChecked={isChecked}
              onChange={this.onChangeCheckbox}
              label={t("Remember")}
            />
            <HelpButton
              className="login-tooltip"
              helpButtonHeaderContent={t("CookieSettingsTitle")}
              tooltipContent={
                <Text fontSize='12px'>{t("RememberHelper")}</Text>
              }
            />
          </div>

          <Link
            fontSize='12px'
            className="login-link"
            type="page"
            isHovered={true}
            onClick={this.onClick}
          >
            {t("ForgotPassword")}
          </Link>
        </div>

        {openDialog ? (
          <SubModalDialog
            openDialog={openDialog}
            isLoading={isLoading}
            email={email}
            onChangeEmail={this.onChangeEmail}
            onSendPasswordInstructions={this.onSendPasswordInstructions}
            onDialogClose={this.onDialogClose}
            t={t}
          />
        ) : null}

        <Button
          id="button"
          className="login-button"
          primary
          size="big"
          label={isLoading ? t("LoadingProcessing") : t("LoginButton")}
          tabIndex={3}
          isDisabled={isLoading}
          isLoading={isLoading}
          onClick={this.onSubmit}
        />

        {params.confirmedEmail && (
          <Text isBold={true} fontSize='16px'>
            {t("MessageEmailConfirmed")} {t("MessageAuthorize")}
          </Text>
        )}
        <Text fontSize='14px' color="#c30">
          {errorText}
        </Text>
      </FormContainer>
    );
  }
}

Form.propTypes = {
  login: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  setIsLoaded: PropTypes.func.isRequired,
  greetingTitle: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
  i18n: PropTypes.object.isRequired,
  language: PropTypes.string.isRequired
};

Form.defaultProps = {
  identifier: "",
  password: "",
  email: ""
};

const FormWrapper = withTranslation()(Form);

const LoginForm = props => {
  const { language, isLoaded } = props;

  i18n.changeLanguage(language);

  return (
    <>
      {isLoaded && <PageLayout sectionBodyContent={<FormWrapper i18n={i18n} {...props} />} />}
    </>
  );
};

LoginForm.propTypes = {
  language: PropTypes.string.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    isLoaded: state.auth.isLoaded,
    language: state.auth.user.cultureName || state.auth.settings.culture,
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(mapStateToProps, { login, setIsLoaded })(
  withRouter(LoginForm)
);
