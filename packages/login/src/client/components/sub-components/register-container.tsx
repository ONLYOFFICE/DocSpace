import React, { useState } from "react";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import toastr from "@docspace/components/toast/toastr";
import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import { sendRegisterRequest } from "@docspace/common/api/settings";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { Base } from "@docspace/components/themes";

interface IRegisterProps {
  language?: string;
  isAuthenticated?: boolean;
  enabledJoin: boolean;
  trustedDomainsType?: number;
  trustedDomains?: string[];
  theme?: any;
  currentColorScheme?: ITheme;
  id?: string;
}

const StyledRegister = styled(Box)`
  position: fixed;
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

const Register: React.FC<IRegisterProps> = (props) => {
  const {
    enabledJoin,
    isAuthenticated,
    trustedDomainsType,
    trustedDomains,
    theme,
    currentColorScheme,
    id,
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

  const onChangeEmail = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e) {
      setEmail(e.currentTarget.value);
      setEmailErr(false);
      setIsShowError(false);
    }
  };

  const onValidateEmail = (res: IEmailValid) => {
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
        .then((res: string) => {
          setLoading(false);
          toastr.success(res);
        })
        .catch((error: any) => {
          setLoading(false);
          toastr.error(error);
        })
        .finally(onRegisterModalClose);
    }
  };

  const onKeyDown = (e: KeyboardEvent) => {
    if (e.key === "Enter") {
      onSendRegisterRequest();
      e.preventDefault();
    }
  };

  return enabledJoin && !isAuthenticated ? (
    <>
      <StyledRegister id={id} onClick={onRegisterClick}>
        <Text as="span" color={currentColorScheme?.main.accent}>
          {t("Register")}
        </Text>
      </StyledRegister>

      {visible && (
        <RegisterModalDialog
          visible={visible}
          loading={loading}
          email={email}
          emailErr={emailErr}
          trustedDomainsType={trustedDomainsType}
          trustedDomains={trustedDomains}
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

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, language } = auth;
  const { theme } = settingsStore;
  return {
    theme,
    isAuthenticated,
    language,
  };
})(observer(Register));
