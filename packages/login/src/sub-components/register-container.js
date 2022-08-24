import React, { useEffect, useState } from "react";

import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import toastr from "@docspace/components/toast/toastr";

import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import PropTypes from "prop-types";
import { sendRegisterRequest } from "@docspace/common/api/settings";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "../i18n";
import { inject, observer } from "mobx-react";
import { Base } from "@docspace/components/themes";

const StyledRegister = styled(Box)`
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 184;
  width: 100%;
  height: 68px;
  padding: 1.5em;
  bottom: 0;
  right: 0;
  background-color: ${(props) => props.theme.login.register.backgroundColor};
  cursor: pointer;
`;

StyledRegister.defaultProps = { theme: Base };

const Register = (props) => {
  const {
    enabledJoin,
    isAuthenticated,
    trustedDomainsType,
    trustedDomains,
    theme,
  } = props;
  const [visible, setVisible] = useState(false);
  const [loading, setLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [isShowError, setIsShowError] = useState(false);

  const { t } = useTranslation("Login");

  const onRegisterClick = () => {
    setVisible(true);
  };

  const onRegisterModalClose = () => {
    setVisible(false);
    setEmail("");
    setEmailErr(false);
  };

  const onChangeEmail = (e) => {
    setEmail(e.currentTarget.value);
    setEmailErr(false);
    setIsShowError(false);
  };

  const onValidateEmail = (res) => {
    setEmailErr(!res.isValid);
    setErrorText(res.errors[0]);
  };

  const onBlurEmail = () => {
    setIsShowError(true);
  };

  const onSendRegisterRequest = () => {
    if (!email.trim() || emailErr) {
      setEmailErr(true);
      setIsShowError(true);
    } else {
      setLoading(true);
      sendRegisterRequest(email)
        .then((res) => {
          setLoading(false);
          toastr.success(res);
        })
        .catch((error) => {
          setLoading(false);
          toastr.error(error);
        })
        .finally(onRegisterModalClose);
    }
  };

  const onKeyDown = (e) => {
    if (e.key === "Enter") {
      onSendRegisterRequest();
      e.preventDefault();
    }
  };

  return enabledJoin && !isAuthenticated ? (
    <>
      <StyledRegister onClick={onRegisterClick}>
        <Text color={theme.login.register.textColor}>{t("Register")}</Text>
      </StyledRegister>

      {visible && (
        <RegisterModalDialog
          visible={visible}
          loading={loading}
          email={email}
          emailErr={emailErr}
          trustedDomainsType={trustedDomainsType}
          trustedDomains={trustedDomains}
          t={t}
          onChangeEmail={onChangeEmail}
          onValidateEmail={onValidateEmail}
          onBlurEmail={onBlurEmail}
          onRegisterModalClose={onRegisterModalClose}
          onSendRegisterRequest={onSendRegisterRequest}
          onKeyDown={onKeyDown}
          errorText={errorText}
          isShowError={isShowError}
        />
      )}
    </>
  ) : (
    <></>
  );
};

Register.propTypes = {
  language: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  enabledJoin: PropTypes.bool,
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, language } = auth;
  const {
    enabledJoin,
    trustedDomainsType,
    trustedDomains,
    theme,
  } = settingsStore;
  return {
    theme,
    enabledJoin,
    trustedDomainsType,
    trustedDomains,
    isAuthenticated,
    language,
  };
})(observer(Register));
