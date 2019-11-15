import React, { useState, useEffect, useCallback } from "react";
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
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { sendInstructionsToChangePassword } from "../../../store/services/api";
import SubModalDialog from "./sub-components/modal-dialog";
import { store } from 'asc-web-common';
const { login } = store.auth.actions;

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

const mdOptions = { size: 6, offset: 3 };

const Form = props => {
  const { t } = useTranslation("translation", { i18n });
  const { login, match, history, language, greetingTitle } = props;
  const { params } = match;
  const [identifier, setIdentifier] = useState(params.confirmedEmail || "");
  const [identifierValid, setIdentifierValid] = useState(true);
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [errorText, setErrorText] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const [openDialog, setOpenDialog] = useState(false);
  const [email, setEmail] = useState("");
  const [isDisabled, setIsDisabled] = useState(false);
  const [isChecked, setIsisChecked] = useState(false);

  const onClick = () => {
    setOpenDialog(true);
    setIsDisabled(true);
    setEmail(identifier);
  };

  const onDialogClose = () => {
    setOpenDialog(false);
    setIsDisabled(false);
    setIsLoading(false);
    setEmail("");
  };

  const onSendPasswordInstructions = useCallback(() => {
    setIsLoading(true);
    sendInstructionsToChangePassword(email)
      .then(res => toastr.success(res), message => toastr.error(message))
      .finally(onDialogClose());
  }, [email]);

  const onSubmit = useCallback(() => {
    errorText && setErrorText("");
    let hasError = false;

    const userName = identifier.trim();

    if (!userName) {
      hasError = true;
      setIdentifierValid(!hasError);
    }

    const pass = password.trim();

    if (!pass) {
      hasError = true;
      setPasswordValid(!hasError);
    }

    if (hasError) return false;

    setIsLoading(true);

    login(userName, pass).then(
      () => {
        //console.log("auth success", match, location, history);
        setIsLoading(false);
        history.push("/");
      },
      error => {
        //console.error("auth error", error);
        setErrorText(error);
        setIsLoading(false);
      }
    );
  }, [errorText, history, identifier, login, password]);

  const onKeyPress = useCallback(
    event => {
      if (event.key === "Enter") {
        !isDisabled ? onSubmit() : onSendPasswordInstructions();
      }
    },
    [onSendPasswordInstructions, onSubmit, isDisabled]
  );

  useEffect(() => {
    i18n.changeLanguage(language);
    params.error && setErrorText(params.error);
    window.addEventListener("keyup", onKeyPress);
    // Remove event listeners on cleanup
    return () => {
      window.removeEventListener("keyup", onKeyPress);
    };
  }, [onKeyPress, params, language]);

  const onChangePassword = event => {
    setPassword(event.target.value);
    !passwordValid && setPasswordValid(true);
    errorText && setErrorText("");
  };

  const onChangeLogin = event => {
    setIdentifier(event.target.value);
    !identifierValid && setIdentifierValid(true);
    errorText && setErrorText("");
  };

  const onChangeEmail = event => {
    setEmail(event.target.value);
  };

  // console.log('Login render');

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
            <CardTitle className="card-title">
              {greetingTitle}
            </CardTitle>
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
            onChange={onChangeLogin}
            onKeyDown={onKeyPress}
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
            onChange={onChangePassword}
            onKeyDown={onKeyPress}
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
            onClick={onClick}
          >
            {t("ForgotPassword")}
          </Link>
          <Checkbox
            className="checkbox"
            isChecked={isChecked}
            onChange={() => setIsisChecked(!isChecked)}
            label={t("Remember")}
          />
          <TooltipStyle>
            <HelpButton
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
          onChangeEmail={onChangeEmail}
          onSendPasswordInstructions={onSendPasswordInstructions}
          onDialogClose={onDialogClose}
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
            onClick={onSubmit}
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
};

const LoginForm = props => (
  <PageLayout sectionBodyContent={<Form {...props} />} />
);

LoginForm.propTypes = {
  login: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired
};

LoginForm.defaultProps = {
  identifier: "",
  password: "",
  email: ""
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture,
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(
  mapStateToProps,
  { login }
)(withRouter(LoginForm));
