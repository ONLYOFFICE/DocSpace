import React, { useCallback, useEffect, useState } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Toast from "@docspace/components/toast";
import FieldContainer from "@docspace/components/field-container";
import SocialButton from "@docspace/components/social-button";
import Section from "@docspace/common/components/Section";
import Register from "./sub-components/register-container";
import {
  getAuthProviders,
  getCapabilities,
} from "@docspace/common/api/settings";
import { getProviderTranslation } from "@docspace/common/utils";
import { providersData } from "@docspace/common/constants";
import { inject, observer } from "mobx-react";
import i18n from "./i18n";
import {
  I18nextProvider,
  useTranslation,
  withTranslation,
} from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import MoreLoginModal from "./sub-components/more-login";
import {
  ButtonsWrapper,
  LoginContainer,
  LoginFormWrapper,
} from "./StyledLogin";
import AppLoader from "@docspace/common/components/AppLoader";
import EmailInput from "@docspace/components/email-input";
import FormWrapper from "@docspace/components/form-wrapper";
import { ReactSVG } from "react-svg";

const Form = (props) => {
  const inputRef = React.useRef(null);

  const [identifierValid, setIdentifierValid] = useState(true);
  const [identifier, setIdentifier] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isDisabled, setIsDisabled] = useState(false);

  const [moreAuthVisible, setMoreAuthVisible] = useState(false);
  const [ssoLabel, setSsoLabel] = useState("");
  const [ssoUrl, setSsoUrl] = useState("");

  const [errorText, setErrorText] = useState("");

  const [isLoaded, setIsLoaded] = useState(false);

  const [isEmailErrorShow, setIsEmailErrorShow] = useState(false);

  const { t } = useTranslation("Login");

  const {
    isDesktop,
    match,
    organizationName,
    greetingTitle,
    thirdPartyLogin,
    providers,
    history,
    setRoomsMode,
  } = props;

  const { error, confirmedEmail } = match.params;

  const oAuthLogin = async (profile) => {
    try {
      await thirdPartyLogin(profile);

      const redirectPath = localStorage.getItem("redirectPath");

      if (redirectPath) {
        localStorage.removeItem("redirectPath");
        window.location.href = redirectPath;
      }
    } catch (e) {
      toastr.error(
        t("Common:ProviderNotConnected"),
        t("Common:ProviderLoginError")
      );
    }

    localStorage.removeItem("profile");
    localStorage.removeItem("code");
  };

  const getSso = async () => {
    const data = await getCapabilities();
    setSsoLabel(data.ssoLabel);
    setSsoUrl(data.ssoUrl);
  };

  useEffect(() => {
    const profile = localStorage.getItem("profile");
    if (!profile) return;

    oAuthLogin(profile);
  }, []);

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
    errorText && setErrorText("");
    setIsEmailErrorShow(false);
  };

  //const throttledKeyPress = throttle(onKeyPress, 500);

  const authCallback = (profile) => {
    oAuthLogin(profile);
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

    Promise.all([setProviders(), getSso()]).then(() => {
      setIsLoaded(true);
      focusInput();
    });

    window.authCallback = authCallback;

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

  const moreAuthOpen = () => {
    setMoreAuthVisible(true);
  };

  const moreAuthClose = () => {
    setMoreAuthVisible(false);
  };

  const onSubmit = () => {
    errorText && setErrorText("");
    let hasError = false;

    const userName = identifier.trim();

    if (!userName) {
      hasError = true;
      setIdentifierValid(false);
    }

    if (hasError) return false;

    history.push("/code"); //TODO: confirm link?
  };

  const onLoginWithPasswordClick = () => {
    setRoomsMode(false);
  };

  const onSocialButtonClick = useCallback((e) => {
    const providerName = e.target.dataset.providername;
    const url = e.target.dataset.url;

    const { getOAuthToken, getLoginLink } = props;

    try {
      const tokenGetterWin = isDesktop
        ? (window.location.href = url)
        : window.open(
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

  const providerButtons = () => {
    const providerButtons =
      providers &&
      providers.map((item, index) => {
        if (!providersData[item.provider]) return;
        if (index > 1) return;

        const { icon, label, iconOptions, className } = providersData[
          item.provider
        ];

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
              isDisabled={isLoading}
            />
          </div>
        );
      });

    return providerButtons;
  };

  const ssoButton = () => {
    return (
      <div className="buttonWrapper">
        <SocialButton
          iconName="/static/images/sso.react.svg"
          className="socialButton"
          label={ssoLabel || getProviderTranslation("sso", t)}
          onClick={() => (window.location.href = ssoUrl)}
          isDisabled={isLoading}
        />
      </div>
    );
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

  const ssoExists = () => {
    if (ssoUrl) return true;
    else return false;
  };

  const onValidateEmail = (res) => {
    //console.log("onValidateEmail", res);
    setIdentifierValid(res.isValid);
    setErrorText(res.errors[0]);
  };

  const onBlurEmail = () => {
    setIsEmailErrorShow(true);
  };

  //console.log("Login render");

  return (
    <LoginContainer>
      {!isLoaded ? (
        <AppLoader />
      ) : (
        <>
          <ReactSVG
            src="/static/images/docspace.big.react.svg"
            className="logo-wrapper"
          />
          <Text
            fontSize="23px"
            fontWeight={700}
            textAlign="center"
            className="greeting-title"
          >
            {greetingTitle}
          </Text>

          <FormWrapper>
            {ssoExists() && <ButtonsWrapper>{ssoButton()}</ButtonsWrapper>}

            {oauthDataExists() && (
              <>
                <ButtonsWrapper>{providerButtons()}</ButtonsWrapper>
                {providers && providers.length > 2 && (
                  <Link
                    isHovered
                    type="action"
                    fontSize="13px"
                    fontWeight="600"
                    color="#3B72A7"
                    className="more-label"
                    onClick={moreAuthOpen}
                  >
                    {t("Common:ShowMore")}
                  </Link>
                )}
              </>
            )}

            {(oauthDataExists() || ssoExists()) && (
              <div className="line">
                <Text color="#A3A9AE" className="or-label">
                  {t("Or")}
                </Text>
              </div>
            )}

            <form className="auth-form-container">
              <FieldContainer
                isVertical={true}
                labelVisible={false}
                hasError={isEmailErrorShow && !identifierValid}
                errorMessage={
                  errorText
                    ? t(`Common:${errorText}`)
                    : t("Common:RequiredField")
                } //TODO: Add wrong login server error
              >
                <EmailInput
                  id="login"
                  name="login"
                  type="email"
                  hasError={isEmailErrorShow && !identifierValid}
                  value={identifier}
                  placeholder={t("RegistrationEmailWatermark")}
                  size="large"
                  scale={true}
                  isAutoFocussed={true}
                  tabIndex={1}
                  isDisabled={isLoading || !identifierValid}
                  autoComplete="username"
                  onChange={onChangeLogin}
                  onBlur={onBlurEmail}
                  onValidateInput={onValidateEmail}
                  forwardedRef={inputRef}
                />
              </FieldContainer>

              <Button
                id="submit"
                className="login-button"
                primary
                size="medium"
                scale={true}
                label={
                  isLoading
                    ? t("Common:LoadingProcessing")
                    : t("Common:LoginButton")
                }
                tabIndex={1}
                isDisabled={isLoading}
                isLoading={isLoading}
                onClick={onSubmit}
              />

              <div className="login-link">
                <Link
                  fontWeight="600"
                  fontSize="13px"
                  color="#316DAA"
                  type="action"
                  isHovered={true}
                  onClick={onLoginWithPasswordClick}
                >
                  {t("SignInWithPassword")}
                </Link>
              </div>

              {confirmedEmail && (
                <Text isBold={true} fontSize="16px">
                  {t("MessageEmailConfirmed")} {t("MessageAuthorize")}
                </Text>
              )}
            </form>
          </FormWrapper>
          <Toast />

          <MoreLoginModal
            t={t}
            visible={moreAuthVisible}
            onClose={moreAuthClose}
            providers={providers}
            onSocialLoginClick={onSocialButtonClick}
            ssoLabel={ssoLabel}
            ssoUrl={ssoUrl}
          />
        </>
      )}
    </LoginContainer>
  );
};

Form.propTypes = {
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
      <Section>
        <Section.SectionBody>
          <Form {...props} />
        </Section.SectionBody>
      </Section>
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
    thirdPartyLogin,
    setProviders,
    providers,
  } = auth;
  const {
    greetingSettings: greetingTitle,
    organizationName,
    enabledJoin,
    defaultPage,
    isDesktopClient: isDesktop,
    getOAuthToken,
    getLoginLink,
    setRoomsMode,
  } = settingsStore;

  return {
    isAuthenticated,
    isLoaded,
    organizationName,
    greetingTitle,
    enabledJoin,
    defaultPage,
    isDesktop,
    thirdPartyLogin,
    getOAuthToken,
    getLoginLink,
    setProviders,
    providers,
    setRoomsMode,
  };
})(withRouter(observer(withTranslation(["Login", "Common"])(LoginForm))));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <Login {...props} />
  </I18nextProvider>
);
