import React, { useState, useEffect, useCallback } from "react";
import { withRouter } from "react-router";
import { connect } from 'react-redux';
import PropTypes from "prop-types";
import styled from "styled-components";
import { Row, Col, Card, CardImg, CardTitle } from "reactstrap";
import { Button, TextInput, PageLayout, Text } from "asc-web-components";
import { useTranslation } from "react-i18next";
import i18n from "../i18n";
import { welcomePageTitle } from "../../../../helpers/customNames";
import { login } from '../../../../../src/store/auth/actions';

const BodyStyle = styled.div`
  margin-top: 70px;

  p {
    margin-bottom: 5px;
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
`;

const Form = () => {
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  //const { login, match, location, history } = props;
  //const { params } = match;

  const { t } = useTranslation("translation", { i18n });

  const onSubmit = () => {
    console.log("onSubmit CHANGE");
  };

  const onKeyPress = useCallback(target => {
    console.log(target.code);
    if (target.code === "Enter") {
      onSubmit();
    }
  });
  //}, [onSubmit]);

  /*
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
  */

  const mdOptions = { size: 6, offset: 3 };
  return (
    <BodyStyle>
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
              {t("CustomWelcomePageTitle", { welcomePageTitle })}
            </CardTitle>
          </Card>

          <Text.Body fontSize={14}>{t("PassworResetTitle")}</Text.Body>
          <TextInput
            id="password"
            name="password"
            type="password"
            size="huge"
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
            autocomple="current-password"
            placeholder={t("PasswordCustomMode")}
            onChange={event => {
              setPassword(event.target.value);
              !passwordValid && setPasswordValid(true);
              errorText && setErrorText("");
              onKeyPress(event.target);
            }}
            value={password}
            hasError={!passwordValid}
            isDisabled={isLoading}
            onKeyDown={event => onKeyPress(event.target)}
          />
        </Col>
      </Row>
      <Row className="login-row">
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
  return <PageLayout sectionBodyContent={<Form />} />;
};

export default connect(null, { login })(withRouter(ChangePasswordForm));
