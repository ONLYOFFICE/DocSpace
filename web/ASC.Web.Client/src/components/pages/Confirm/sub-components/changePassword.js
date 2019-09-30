import React, { useState, useEffect, useCallback } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Row, Col, Card, CardImg, CardTitle } from "reactstrap";
import { Button, PageLayout, Text, PasswordInput } from "asc-web-components";
import { useTranslation } from "react-i18next";
import i18n from "../i18n";
import { welcomePageTitle } from "../../../../helpers/customNames";
import {
  changePassword,
  getPasswordSettings
} from "../../../../../src/store/auth/actions";

const BodyStyle = styled.div`
  margin-top: 70px;

  p {
    margin-bottom: 5px;
  }

  .password-row {
    margin: 23px 0 0;

    .password-card {
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
`;

const Form = props => {
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const {
    match,
    location,
    history,
    changePassword,
    getPasswordSettings
  } = props;
  const { params } = match;
  const { t } = useTranslation("translation", { i18n });

  const indexOfSlash = match.path.lastIndexOf("/");
  const typeLink = match.path.slice(indexOfSlash + 1);
  const queryString = `type=${typeLink}&${location.search.slice(1)}`;

  getPasswordSettings(queryString)
    .then(function() {
      console.log("GET PASSWORD SETTINGS SUCCESS");
    })
    .catch(e => {
      console.log("ERROR GET PASSWORD SETTINGS", e);
      history.push(`/login/error=${e}`);
    });

  const onSubmit = useCallback(
    e => {
      errorText && setErrorText("");

      let hasError = false;

      if (!password.trim()) {
        hasError = true;
        setPasswordValid(!hasError);
      }

      if (hasError) return false;

      setIsLoading(true);
      console.log("changePassword onSubmit", match, location, history);

      const str = location.search.split("&");
      const userId = str[2].slice(4);
      const key = `type=PasswordChange&${location.search.slice(1)}`;

      changePassword(userId, { password }, key)
        .then(() => {
          console.log("UPDATE PASSWORD");
          history.push("/");
        })
        .catch(e => {
          history.push("/");
          console.log("ERROR UPDATE PASSWORD", e);
        });
    },
    [errorText, history, location, changePassword, match, password]
  );

  const onKeyPress = useCallback(
    target => {
      if (target.key === "Enter") {
        onSubmit();
      }
    },
    [onSubmit]
  );

  useEffect(() => {
    params.error && setErrorText(params.error);
    window.addEventListener("keydown", onKeyPress);
    window.addEventListener("keyup", onKeyPress);
    // Remove event listeners on cleanup
    return () => {
      window.removeEventListener("keydown", onKeyPress);
      window.removeEventListener("keyup", onKeyPress);
    };
  }, [onKeyPress, params.error]);

  const settings = {
    minLength: 6,
    upperCase: false,
    digits: false,
    specSymbols: false
  };

  const tooltipPasswordLength =
    "from " + settings.minLength + " to 30 characters";

  const mdOptions = { size: 6, offset: 3 };
  return (
    <BodyStyle>
      <Row className="password-row">
        <Col sm="12" md={mdOptions}>
          <Card className="password-card">
            <CardImg
              className="card-img"
              src="images/dark_general.png"
              alt="Logo"
              top
            />
            <CardTitle className="card-title">
              {t("CustomWelcomePageTitle", { welcomePageTitle })}
            </CardTitle>
          </Card>

          <Text.Body fontSize={14}>{t("PassworResetTitle")}</Text.Body>
          <PasswordInput
            id="password"
            name="password"
            size="huge"
            scale={true}
            type="password"
            isDisabled={isLoading}
            hasError={!passwordValid}
            tabIndex={1}
            value={password}
            onChange={event => {
              setPassword(event.target.value);
              !passwordValid && setPasswordValid(true);
              errorText && setErrorText("");
              onKeyPress(event.target);
            }}
            emailInputName="E-mail"
            passwordSettings={settings}
            tooltipPasswordTitle="Password must contain:"
            tooltipPasswordLength={tooltipPasswordLength}
            placeholder={t("PasswordCustomMode")}
            maxLength={30}

            //isAutoFocussed={true}
            //autocomple="current-password"
            //onKeyDown={event => onKeyPress(event.target)}
          />
        </Col>
      </Row>
      <Row className="password-row">
        <Col sm="12" md={mdOptions}>
          <Button
            primary
            size="big"
            tabIndex={3}
            label={
              isLoading ? t("LoadingProcessing") : t("ImportContactsOkButton")
            }
            isDisabled={isLoading}
            isLoading={isLoading}
            onClick={onSubmit}
          />
        </Col>
      </Row>
    </BodyStyle>
  );
};

const ChangePasswordForm = props => {
  return <PageLayout sectionBodyContent={<Form {...props} />} />;
};

ChangePasswordForm.propTypes = {
  match: PropTypes.object.isRequired,
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  changePassword: PropTypes.func.isRequired
};

ChangePasswordForm.defaultProps = {
  password: ""
};

export default connect(
  null,
  { changePassword, getPasswordSettings }
)(withRouter(withTranslation()(ChangePasswordForm)));
