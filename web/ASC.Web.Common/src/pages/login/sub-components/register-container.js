import React, { useEffect, useState } from "react";
import { Box, Text, toastr } from "asc-web-components";
import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import PropTypes from "prop-types";
import { sendRegisterRequest } from "../../../api/settings/index";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "../i18n";
import { inject, observer } from "mobx-react";

const StyledRegister = styled(Box)`
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 184;
  width: 100%;
  height: 66px;
  padding: 1.5em;
  bottom: 0;
  right: 0;
  background-color: #f8f9f9;
  cursor: pointer;
`;

const Register = ({ t }) => {
  const [visible, setVisible] = useState(false);
  const [loading, setLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);

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
  };
  const onSendRegisterRequest = () => {
    if (!email.trim()) {
      setEmailErr(true);
    } else {
      setLoading(true);
      sendRegisterRequest(email)
        .then(() => {
          setLoading(false);
          toastr.success("Successfully sent");
        })
        .catch((error) => {
          setLoading(false);
          toastr.error(error);
        })
        .finally(onRegisterModalClose);
    }
  };

  return (
    <>
      <StyledRegister onClick={onRegisterClick}>
        <Text color="#316DAA">{t("Register")}</Text>
      </StyledRegister>

      {visible && (
        <RegisterModalDialog
          visible={visible}
          loading={loading}
          email={email}
          emailErr={emailErr}
          t={t}
          onChangeEmail={onChangeEmail}
          onRegisterModalClose={onRegisterModalClose}
          onSendRegisterRequest={onSendRegisterRequest}
        />
      )}
    </>
  );
};

Register.propTypes = {
  t: PropTypes.func.isRequired,
};

const RegisterTranslationWrapper = withTranslation()(Register);

const RegisterWrapper = (props) => {
  const { language, isAuthenticated, enabledJoin } = props;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return (
    <I18nextProvider i18n={i18n}>
      {enabledJoin && !isAuthenticated && (
        <RegisterTranslationWrapper {...props} />
      )}
    </I18nextProvider>
  );
};

RegisterWrapper.propTypes = {
  language: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  enabledJoin: PropTypes.bool,
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, language } = auth;
  const { enabledJoin } = settingsStore;
  return {
    enabledJoin,
    isAuthenticated,
    language,
  };
})(observer(RegisterWrapper));
