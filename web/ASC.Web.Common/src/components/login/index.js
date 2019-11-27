import React, { Component } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  Collapse,
  Container,
  Row,
  Col,
  Card,
  CardTitle,
  CardImg
} from "reactstrap";
import {
  Button,
  TextInput,
  PageLayout,
  Text,
  Link,
  toastr,
  Checkbox,
  HelpButton
} from "asc-web-components";
import { connect } from "react-redux";
import styled from "styled-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import SubModalDialog from "./sub-components/modal-dialog";
import { login, setIsLoaded } from "../../store/auth/actions";
import { sendInstructionsToChangePassword } from "../../api/people";

const FormContainer = styled(Container)`
  margin-top: 70px;

  .link-style {
    float: right;
    line-height: 16px;
  }

  .text-body {
    margin-bottom: 16px;
  }

  .btn-style {
    margin-right: 8px;
  }

  .checkbox {
    float: left;
    span {
      font-size: 12px;
    }
  }

  .question-icon {
    float: left;
    margin-left: 4px;
    line-height: 16px;
  }

  .login-row {
    margin: 23px 0 0;

    .login-card {
      border: none;

      .card-img {
        max-width: 216px;
        max-height: 35px;
      }
      .card-title {
        word-wrap: break-word;
        margin: 8px 0;
        text-align: left;
        font-size: 24px;
        color: #116d9d;
      }
    }
  }

  .button-row {
    margin: 16px 0 0;
  }
`;

const TooltipStyle = styled.span`
  margin-left: 3px;
  position: absolute;
  margin-top: 2px;
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

    login(userName, pass).then(
      () => {
        this.setState({ isLoading: false });
        setIsLoaded(true);
        history.push("/");
      },
      error => {
        this.setState({ errorText: error, isLoading: false });
      }
    );
  };

  componentDidMount() {
    const { language, match } = this.props;
    const { params } = match;
    i18n.changeLanguage(language);
    params.error && this.setState({ errorText: params.error });
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
  }

  render() {
    const mdOptions = { size: 6, offset: 3 };
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
        <Row className="login-row">
          <Col sm="12" md={mdOptions}>
            <Card className="login-card">
              <CardImg
                className="card-img"
                src="images/dark_general.png"
                alt="Logo"
                top
              />
              <CardTitle className="card-title">{greetingTitle}</CardTitle>
            </Card>
          </Col>
        </Row>
        <Row className="login-row">
          <Col sm="12" md={mdOptions}>
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
            />
          </Col>
        </Row>
        <Row className="login-row">
          <Col sm="12" md={mdOptions}>
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
            />
          </Col>
        </Row>
        <Row className="login-row">
          <Col sm="12" md={mdOptions}>
            <Link
              fontSize={12}
              className="link-style"
              type="page"
              isHovered={true}
              onClick={this.onClick}
            >
              {t("ForgotPassword")}
            </Link>
            <Checkbox
              className="checkbox"
              isChecked={isChecked}
              onChange={this.onChangeCheckbox}
              label={t("Remember")}
            />
            <TooltipStyle>
              <HelpButton
                helpButtonHeaderContent={t("CookieSettingsTitle")}
                tooltipContent={
                  <Text.Body fontSize={12}>{t("RememberHelper")}</Text.Body>
                }
              />
            </TooltipStyle>
          </Col>
        </Row>

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

        <Row className="button-row">
          <Col sm="12" md={mdOptions}>
            <Button
              primary
              size="big"
              label={isLoading ? t("LoadingProcessing") : t("LoginButton")}
              tabIndex={3}
              isDisabled={isLoading}
              isLoading={isLoading}
              onClick={this.onSubmit}
            />
          </Col>
        </Row>
        {params.confirmedEmail && (
          <Row className="login-row">
            <Col sm="12" md={mdOptions}>
              <Text.Body isBold={true} fontSize={16}>
                {t("MessageEmailConfirmed")} {t("MessageAuthorize")}
              </Text.Body>
            </Col>
          </Row>
        )}
        <Collapse isOpen={!!errorText}>
          <Row className="login-row">
            <Col sm="12" md={mdOptions}>
              <div className="alert alert-danger">{errorText}</div>
            </Col>
          </Row>
        </Collapse>
      </FormContainer>
    );
  }
}

const FormWrapper = withTranslation()(Form);

const LoginForm = props => {
  const { language } = props;
  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <PageLayout sectionBodyContent={<FormWrapper {...props} />} />
    </I18nextProvider>
  );
};

Form.propTypes = {
  login: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  setIsLoaded: PropTypes.func.isRequired,
  greetingTitle: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
  language: PropTypes.string.isRequired
};

Form.defaultProps = {
  identifier: "",
  password: "",
  email: ""
};

LoginForm.propTypes = {
  language: PropTypes.string.isRequired
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture,
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(mapStateToProps, { login, setIsLoaded })(
  withRouter(LoginForm)
);
