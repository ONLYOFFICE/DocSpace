import React, { useEffect, useState } from "react";

import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import toastr from "@appserver/components/toast/toastr";

import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import PropTypes from "prop-types";
import { sendRegisterRequest } from "@appserver/common/api/settings";
import { I18nextProvider, useTranslation } from "react-i18next";
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

const Register = (props) => {
  const {
    enabledJoin,
    isAuthenticated,
    trustedDomainsType,
    trustedDomains,
  } = props;
  const [visible, setVisible] = useState(false);
  const [loading, setLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);

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
  };
  const onSendRegisterRequest = () => {
    if (!email.trim()) {
      setEmailErr(true);
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

  return enabledJoin && !isAuthenticated ? (
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
          trustedDomainsType={trustedDomainsType}
          trustedDomains={trustedDomains}
          t={t}
          onChangeEmail={onChangeEmail}
          onRegisterModalClose={onRegisterModalClose}
          onSendRegisterRequest={onSendRegisterRequest}
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
  const { enabledJoin, trustedDomainsType, trustedDomains } = settingsStore;
  return {
    enabledJoin,
    trustedDomainsType,
    trustedDomains,
    isAuthenticated,
    language,
  };
})(observer(Register));
