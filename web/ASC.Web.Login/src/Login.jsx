import React, { useCallback, useEffect, useState } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import Toast from "@appserver/components/toast";
import HelpButton from "@appserver/components/help-button";
import PasswordInput from "@appserver/components/password-input";
import FieldContainer from "@appserver/components/field-container";
import SocialButton from "@appserver/components/social-button";
import FacebookButton from "@appserver/components/facebook-button";
import PageLayout from "@appserver/common/components/PageLayout";
import ForgotPasswordModalDialog from "./sub-components/forgot-password-modal-dialog";
import Register from "./sub-components/register-container";
import { getAuthProviders } from "@appserver/common/api/settings";
import { checkPwd } from "@appserver/common/desktop";
import {
  createPasswordHash,
  getProviderTranslation,
} from "@appserver/common/utils";
import { providersData } from "@appserver/common/constants";
import { inject, observer } from "mobx-react";
import i18n from "./i18n";
import {
  I18nextProvider,
  useTranslation,
  withTranslation,
} from "react-i18next";
import toastr from "@appserver/components/toast/toastr";

const ButtonsWrapper = styled.div`
  display: table;
  margin: auto;
`;

const LoginContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 120px auto 0 auto;
  max-width: 960px;

  .login-tooltip {
    padding-left: 4px;
    display: inline-block;
  }

  .buttonWrapper {
    margin: 6px;
    min-width: 225px;
  }

  .line {
    height: 1px;
    background-color: #eceef1;
    flex-basis: 100%;
    margin: 0 8px;
  }

  @media (max-width: 768px) {
    padding: 0 16px;
    max-width: 475px;
  }
  @media (max-width: 375px) {
    margin: 72px auto 0 auto;
    max-width: 311px;
  }

  .greeting-title {
    width: 100%;

    @media (max-width: 768px) {
      text-align: left;
    }
    @media (max-width: 375px) {
      font-size: 23px;
    }
  }

  .auth-form-container {
    margin: 32px 213px 0 213px;
    width: 311px;

    @media (max-width: 768px) {
      margin: 32px 0 0 0;
      width: 100%;
    }
    @media (max-width: 375px) {
      margin: 32px 0 0 0;
      width: 100%;
    }

    .login-forgot-wrapper {
      height: 36px;
      padding: 14px 0;

      .login-checkbox-wrapper {
        display: flex;

        .login-checkbox {
          float: left;
          span {
            font-size: 12px;
          }
        }
      }

      .login-link {
        line-height: 18px;
        margin-left: auto;
      }
    }

    .login-button {
      margin-bottom: 16px;
    }

    .login-button-dialog {
      margin-right: 8px;
    }

    .login-bottom-border {
      width: 100%;
      height: 1px;
      background: #eceef1;
    }

    .login-bottom-text {
      margin: 0 8px;
    }
  }
`;

const LoginFormWrapper = styled.div`
  display: grid;
  grid-template-rows: ${(props) =>
    props.enabledJoin
      ? props.isDesktop
        ? css`1fr 10px`
        : css`1fr 66px`
      : css`1fr`};
  width: 100%;
  height: calc(100vh-56px);
`;

const settings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false,
};

const Form = (props) => {
  const inputRef = React.useRef(null);

  const [identifierValid, setIdentifierValid] = useState(true);
  const [identifier, setIdentifier] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isDisabled, setIsDisabled] = useState(false);

  const [passwordValid, setPasswordValid] = useState(true);
  const [password, setPassword] = useState("");
  const [isChecked, setIsChecked] = useState(false);
  const [isDialogVisible, setIsDialogVisible] = useState(false);

  const [errorText, setErrorText] = useState("");

  const { t } = useTranslation("Login");

  const {
    login,
    hashSettings,
    isDesktop,
    defaultPage,
    match,
    organizationName,
    greetingTitle,
    history,
    thirdPartyLogin,
    providers,
  } = props;

  const { error, confirmedEmail } = match.params;

  const onKeyDown = (e) => {
    //console.log("onKeyDown", e.key);
    if (e.key === "Enter") {
      onClearErrors(e);
      !isDisabled && onSubmit(e);
      e.preventDefault();
    }
  };

  const onClearErrors = (e) => {
    //console.log("onClearErrors", e);
    !identifierValid && setIdentifierValid(true);
    !passwordValid && setPasswordValid(true);
    errorText && setErrorText("");
  };

  //const throttledKeyPress = throttle(onKeyPress, 500);

  const authCallback = (profile) => {
    thirdPartyLogin(profile.Serialized)
      .then(() => {
        setIsLoading(true);
        history.push(defaultPage);
      })
      .catch(() => {
        toastr.error(
          t("Common:ProviderNotConnected"),
          t("Common:ProviderLoginError")
        );
      });
  };

  const setProviders = async () => {
    const { setProviders } = props;

    try {
      await getAuthProviders().then((providers) => {
        setProviders(providers);
      });
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(async () => {
    document.title = `${t("Authorization")} – ${organizationName}`; //TODO: implement the setDocumentTitle() utility in ASC.Web.Common

    error && setErrorText(error);
    confirmedEmail && setIdentifier(confirmedEmail);

    focusInput();

    window.authCallback = authCallback;

    await setProviders();
    //window.addEventListener("keyup", throttledKeyPress, false);

    /*return () => {
      window.removeEventListener("keyup", throttledKeyPress, false);
    };*/
  }, []);

  const focusInput = () => {
    if (inputRef) {
      inputRef.current.focus();
    }
  };

  const onChangeLogin = (e) => {
    //console.log("onChangeLogin", e.target.value);
    setIdentifier(e.target.value);
    onClearErrors(e);
  };

  const onChangePassword = (e) => {
    //console.log("onChangePassword", e.target.value);
    setPassword(e.target.value);
    onClearErrors(e);
  };

  const onChangeCheckbox = () => setIsChecked(!isChecked);

  const onClick = () => {
    setIsDialogVisible(true);
    setIsDisabled(true);
  };

  const onDialogClose = () => {
    setIsDialogVisible(false);
    setIsDisabled(false);
    setIsLoading(false);
  };

  const onSubmit = () => {
    errorText && setErrorText("");
    let hasError = false;

    const userName = identifier.trim();

    if (!userName) {
      hasError = true;
      setIdentifierValid(false);
    }

    const pass = password.trim();

    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (hasError) return false;

    setIsLoading(true);
    const hash = createPasswordHash(pass, hashSettings);

    isDesktop && checkPwd();
    login(userName, hash)
      .then((res) => {
        const { url, user, hash } = res;
        history.push(url, { user, hash });
      })
      .catch((error) => {
        setErrorText(error);
        setIdentifierValid(!error);
        setPasswordValid(!error);
        setIsLoading(false);
        focusInput();
      });
  };

  const onSocialButtonClick = useCallback((e) => {
    const providerName = e.target.dataset.providername;
    const url = e.target.dataset.url;

    const { getOAuthToken, getLoginLink } = props;

    try {
      const tokenGetterWin = window.open(
        url,
        "login",
        "width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no"
      );

      getOAuthToken(tokenGetterWin).then((code) => {
        const token = window.btoa(
          JSON.stringify({
            auth: providerName,
            mode: "popup",
            callback: "authCallback",
          })
        );

        tokenGetterWin.location.href = getLoginLink(token, code);
      });
    } catch (err) {
      console.log(err);
    }
  }, []);

  const addFacebookToStart = (facebookIndex, providerButtons) => {
    const faceBookData = providers[facebookIndex];
    const { icon, label, iconOptions } = providersData[faceBookData.provider];
    providerButtons.unshift(
      <div
        className="buttonWrapper"
        key={`${faceBookData.provider}ProviderItem`}
      >
        <FacebookButton
          iconName={icon}
          label={getProviderTranslation(label, t)}
          className="socialButton"
          $iconOptions={iconOptions}
          data-url={faceBookData.url}
          data-providername={faceBookData.provider}
          onClick={onSocialButtonClick}
        />
      </div>
    );
  };

  const providerButtons = () => {
    let facebookIndex = null;

    const providerButtons =
      providers &&
      providers.map((item, index) => {
        if (!providersData[item.provider]) return;

        const { icon, label, iconOptions, className } = providersData[
          item.provider
        ];

        if (item.provider === "facebook") {
          facebookIndex = index;
          return;
        }
        return (
          <div className="buttonWrapper" key={`${item.provider}ProviderItem`}>
            <SocialButton
              iconName={icon}
              label={getProviderTranslation(label, t)}
              className={`socialButton ${className ? className : ""}`}
              $iconOptions={iconOptions}
              data-url={item.url}
              data-providername={item.provider}
              onClick={onSocialButtonClick}
            />
          </div>
        );
      });

    if (facebookIndex) addFacebookToStart(facebookIndex, providerButtons);

    return providerButtons;
  };

  const oauthDataExists = () => {
    let existProviders = 0;
    providers && providers.length > 0;
    providers.map((item) => {
      if (!providersData[item.provider]) return;
      existProviders++;
    });

    return !!existProviders;
  };

  //console.log("Login render");
  
  return (
    <LoginContainer>
      <Text
        fontSize="32px"
        fontWeight={600}
        textAlign="center"
        className="greeting-title"
      >
        {greetingTitle}
      </Text>

      <form className="auth-form-container">
        <FieldContainer
          isVertical={true}
          labelVisible={false}
          hasError={!identifierValid}
          errorMessage={errorText ? errorText : t("Common:RequiredField")} //TODO: Add wrong login server error
        >
          <TextInput
            id="login"
            name="login"
            type="email"
            hasError={!identifierValid}
            value={identifier}
            placeholder={t("RegistrationEmailWatermark")}
            size="large"
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
            isDisabled={isLoading}
            autoComplete="username"
            onChange={onChangeLogin}
            onKeyDown={onKeyDown}
            forwardedRef={inputRef}
          />
        </FieldContainer>
        <FieldContainer
          isVertical={true}
          labelVisible={false}
          hasError={!passwordValid}
          errorMessage={errorText ? "" : t("Common:RequiredField")} //TODO: Add wrong password server error
        >
          <PasswordInput
            simpleView={true}
            passwordSettings={settings}
            id="password"
            inputName="password"
            placeholder={t("Common:Password")}
            type="password"
            hasError={!passwordValid}
            inputValue={password}
            size="large"
            scale={true}
            tabIndex={1}
            isDisabled={isLoading}
            autoComplete="current-password"
            onChange={onChangePassword}
            onKeyDown={onKeyDown}
          />
        </FieldContainer>
        <div className="login-forgot-wrapper">
          <div className="login-checkbox-wrapper">
            <Checkbox
              className="login-checkbox"
              isChecked={isChecked}
              onChange={onChangeCheckbox}
              label={<Text fontSize="13px">{t("Remember")}</Text>}
            />
            <HelpButton
              className="login-tooltip"
              helpButtonHeaderContent={t("CookieSettingsTitle")}
              tooltipContent={
                <Text fontSize="12px">{t("RememberHelper")}</Text>
              }
            />
            <Link
              fontSize="13px"
              color="#316DAA"
              className="login-link"
              type="page"
              isHovered={false}
              onClick={onClick}
            >
              {t("ForgotPassword")}
            </Link>
          </div>
        </div>

        {isDialogVisible && (
          <ForgotPasswordModalDialog
            visible={isDialogVisible}
            email={identifier}
            onDialogClose={onDialogClose}
          />
        )}

        <Button
          id="submit"
          className="login-button"
          primary
          size="large"
          scale={true}
          label={isLoading ? t("Common:LoadingProcessing") : t("LoginButton")}
          tabIndex={1}
          isDisabled={isLoading}
          isLoading={isLoading}
          onClick={onSubmit}
        />

        {confirmedEmail && (
          <Text isBold={true} fontSize="16px">
            {t("MessageEmailConfirmed")} {t("MessageAuthorize")}
          </Text>
        )}

        {oauthDataExists() && (
          <>
            <Box displayProp="flex" alignItems="center" marginProp="0 0 16px 0">
              <div className="login-bottom-border"></div>
              <Text className="login-bottom-text" color="#A3A9AE">
                {t("Or")}
              </Text>
              <div className="login-bottom-border"></div>
            </Box>

            <ButtonsWrapper>{providerButtons()}</ButtonsWrapper>
          </>
        )}
      </form>
      <Toast />
    </LoginContainer>
  );
};

Form.propTypes = {
  login: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  hashSettings: PropTypes.object,
  greetingTitle: PropTypes.string.isRequired,
  organizationName: PropTypes.string,
  homepage: PropTypes.string,
  defaultPage: PropTypes.string,
  isDesktop: PropTypes.bool,
};

Form.defaultProps = {
  identifier: "",
  password: "",
  email: "",
};

const LoginForm = (props) => {
  const { enabledJoin, isDesktop } = props;

  return (
    <LoginFormWrapper enabledJoin={enabledJoin} isDesktop={isDesktop}>
      <PageLayout>
        <PageLayout.SectionBody>
          <Form {...props} />
        </PageLayout.SectionBody>
      </PageLayout>
      <Register />
    </LoginFormWrapper>
  );
};

LoginForm.propTypes = {
  isLoaded: PropTypes.bool,
  enabledJoin: PropTypes.bool,
  isDesktop: PropTypes.bool.isRequired,
};

const Login = inject(({ auth }) => {
  const {
    settingsStore,
    isAuthenticated,
    isLoaded,
    login,
    thirdPartyLogin,
    setProviders,
    providers,
  } = auth;
  const {
    greetingSettings: greetingTitle,
    organizationName,
    hashSettings,
    enabledJoin,
    defaultPage,
    isDesktopClient: isDesktop,
    getOAuthToken,
    getLoginLink,
  } = settingsStore;

  return {
    isAuthenticated,
    isLoaded,
    organizationName,
    greetingTitle,
    hashSettings,
    enabledJoin,
    defaultPage,
    isDesktop,
    login,
    thirdPartyLogin,
    getOAuthToken,
    getLoginLink,
    setProviders,
    providers,
  };
})(withRouter(observer(withTranslation(["Login", "Common"])(LoginForm))));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Login {...props} />
  </I18nextProvider>
);
