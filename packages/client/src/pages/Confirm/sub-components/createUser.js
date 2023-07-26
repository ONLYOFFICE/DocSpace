import SsoReactSvgUrl from "PUBLIC_DIR/images/sso.react.svg?url";
import React, { useEffect, useState, useCallback } from "react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { createUser, signupOAuth } from "@docspace/common/api/people";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { isDesktop as isDesktopUtil } from "@docspace/components/utils/device";
import Avatar from "@docspace/components/avatar";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import PasswordInput from "@docspace/components/password-input";
import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import SocialButton from "@docspace/components/social-button";
import { getUserFromConfirm } from "@docspace/common/api/people";
import {
  createPasswordHash,
  getProviderTranslation,
  getOAuthToken,
  getLoginLink,
} from "@docspace/common/utils";
import { providersData } from "@docspace/common/constants";
import withLoader from "../withLoader";
import MoreLoginModal from "@docspace/common/components/MoreLoginModal";
import EmailInput from "@docspace/components/email-input";
import { getPasswordErrorMessage } from "../../../helpers/utils";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";
import DefaultUserPhoto from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";
import { StyledPage, StyledContent } from "./StyledConfirm";
import {
  ButtonsWrapper,
  ConfirmContainer,
  GreetingContainer,
  RegisterContainer,
} from "./StyledCreateUser";

const CreateUserForm = (props) => {
  const {
    settings,
    t,
    greetingTitle,
    providers,
    isDesktop,
    linkData,
    capabilities,
    currentColorScheme,
  } = props;
  const inputRef = React.useRef(null);

  const emailFromLink = linkData?.email ? linkData.email : "";

  const [moreAuthVisible, setMoreAuthVisible] = useState(false);
  const [ssoLabel, setSsoLabel] = useState("");
  const [ssoUrl, setSsoUrl] = useState("");
  const [email, setEmail] = useState(emailFromLink);
  const [emailValid, setEmailValid] = useState(true);
  const [emailErrorText, setEmailErrorText] = useState("");

  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

  const [fname, setFname] = useState("");
  const [fnameValid, setFnameValid] = useState(true);
  const [sname, setSname] = useState("");
  const [snameValid, setSnameValid] = useState(true);

  const [isLoading, setIsLoading] = useState(false);

  const [errorText, setErrorText] = useState("");

  const [user, setUser] = useState("");

  const [isEmailErrorShow, setIsEmailErrorShow] = useState(false);
  const [isPasswordErrorShow, setIsPasswordErrorShow] = useState(false);

  const [showForm, setShowForm] = useState(true);
  const [showGreeting, setShowGreeting] = useState(true);

  const focusInput = () => {
    if (inputRef) {
      inputRef.current.focus();
    }
  };

  const onCheckGreeting = () => {
    const isGreetingMode = oauthDataExists() && isMobile; /*!isDesktopUtil()*/
    setShowForm(!isGreetingMode);
  };

  const onGreetingJoin = () => {
    setShowForm(true);
    setShowGreeting(false);
  };

  /*useEffect(() => {
    window.addEventListener("resize", onCheckGreeting);
    return () => window.removeEventListener("resize", onCheckGreeting);
  }, []);*/

  useEffect(() => {
    const { isAuthenticated, logout, linkData } = props;

    if (isAuthenticated) {
      const path = window.location;
      logout().then(() => window.location.replace(path));
    }

    const fetchData = async () => {
      if (linkData.type === "LinkInvite") {
        const uid = linkData.uid;
        const confirmKey = linkData.confirmHeader;
        const user = await getUserFromConfirm(uid, confirmKey);
        setUser(user);
      }

      window.authCallback = authCallback;

      setSsoLabel(capabilities?.ssoLabel);
      setSsoUrl(capabilities?.ssoUrl);
      onCheckGreeting();
      focusInput();
    };

    fetchData();
  }, [props.isAuthenticated]);

  const onSubmit = () => {
    const { defaultPage, linkData, hashSettings } = props;
    const type = parseInt(linkData.emplType);

    setIsLoading(true);

    setErrorText("");

    let hasError = false;

    if (!fname.trim()) {
      hasError = true;
      setFnameValid(!hasError);
    }

    if (!sname.trim()) {
      hasError = true;
      setSnameValid(!hasError);
    }

    const emailRegex = "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$";
    const validationEmail = new RegExp(emailRegex);

    if (!validationEmail.test(email.trim())) {
      hasError = true;
      setEmailValid(!hasError);
    }

    if (!passwordValid || !password.trim()) {
      hasError = true;
      setPasswordValid(!hasError);
      setIsPasswordErrorShow(true);
    }

    if (hasError) {
      setIsLoading(false);
      return false;
    }

    const hash = createPasswordHash(password, hashSettings);

    const loginData = {
      userName: email,
      passwordHash: hash,
    };

    const personalData = {
      firstname: fname,
      lastname: sname,
      email: email,
    };

    if (!!type) {
      personalData.type = type;
    }

    if (!!linkData.key) {
      personalData.key = linkData.key;
    }

    const headerKey = linkData.confirmHeader;

    createConfirmUser(personalData, loginData, headerKey)
      .then(() => window.location.replace(defaultPage))
      .catch((error) => {
        let errorMessage = "";
        if (typeof error === "object") {
          errorMessage =
            error?.response?.data?.error?.message ||
            error?.statusText ||
            error?.message ||
            "";
        } else {
          errorMessage = error;
        }

        console.error("confirm error", errorMessage);
        setIsEmailErrorShow(true);
        setEmailErrorText(errorMessage);
        setEmailValid(false);
        setIsLoading(false);
      });
  };

  const authCallback = (profile) => {
    const { defaultPage } = props;

    const signupAccount = {
      EmployeeType: linkData.emplType || null,
      SerializedProfile: profile,
    };

    signupOAuth(signupAccount)
      .then(() => {
        window.location.replace(defaultPage);
      })
      .catch((e) => {
        toastr.error(e);
      });
  };

  const createConfirmUser = async (registerData, loginData, key) => {
    const { login } = props;
    const fromInviteLink = linkData.type === "LinkInvite" ? true : false;

    const data = Object.assign(
      { fromInviteLink: fromInviteLink },
      registerData,
      loginData
    );

    const user = await createUser(data, key);

    const { userName, passwordHash } = loginData;

    const response = await login(userName, passwordHash);

    return user;
  };

  const moreAuthOpen = () => {
    setMoreAuthVisible(true);
  };

  const moreAuthClose = () => {
    setMoreAuthVisible(false);
  };

  const onChangeEmail = (e) => {
    setEmail(e.target.value);
    setIsEmailErrorShow(false);
  };

  const onChangeFname = (e) => {
    setFname(e.target.value);
    setFnameValid(true);
    setErrorText("");
  };

  const onChangeSname = (e) => {
    setSname(e.target.value);
    setSnameValid(true);
    setErrorText("");
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
    setErrorText("");
    setIsPasswordErrorShow(false);
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSubmit();
    }
  };

  const onSocialButtonClick = useCallback((e) => {
    const { target } = e;
    let targetElement = target;

    if (!(targetElement instanceof HTMLButtonElement) && target.parentElement) {
      targetElement = target.parentElement;
    }

    const providerName = targetElement.dataset.providername;
    const url = targetElement.dataset.url || "";

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

        const { icon, label, iconOptions, className } =
          providersData[item.provider];

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

    return providerButtons;
  };

  const ssoButton = () => {
    return (
      <div className="buttonWrapper">
        <SocialButton
          iconName={SsoReactSvgUrl}
          className="socialButton"
          label={ssoLabel || getProviderTranslation("sso", t)}
          onClick={() => (window.location.href = ssoUrl)}
        />
      </div>
    );
  };

  const oauthDataExists = () => {
    if (!capabilities?.oauthEnabled) return false;

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
    setEmailValid(res.isValid);
    setEmailErrorText(res.errors[0]);
  };

  const onValidatePassword = (res) => {
    setPasswordValid(res);
  };

  const onBlurEmail = () => {
    setIsEmailErrorShow(true);
  };

  const onBlurPassword = () => {
    setIsPasswordErrorShow(true);
  };

  const userAvatar = user && user.hasAvatar ? user.avatar : DefaultUserPhoto;

  return (
    <StyledPage>
      <StyledContent>
        <ConfirmContainer>
          <GreetingContainer>
            <DocspaceLogo className="docspace-logo" />
            <Text
              fontSize="23px"
              fontWeight={700}
              textAlign="left"
              className="greeting-title"
            >
              {greetingTitle}
            </Text>

            {showGreeting && (
              <>
                {user && (
                  <div className="greeting-block">
                    <Avatar
                      className="avatar"
                      role="user"
                      source={userAvatar}
                    />
                    <div className="user-info">
                      <Text fontSize="15px" fontWeight={600}>
                        {user.firstName} {user.lastName}
                      </Text>
                      <Text fontSize="12px" fontWeight={600} color="#A3A9AE">
                        {user.department}
                      </Text>
                    </div>
                  </div>
                )}

                <div className="tooltip">
                  <span className="tooltiptext">{t("WelcomeUser")}</span>
                </div>
              </>
            )}
          </GreetingContainer>

          <FormWrapper>
            <RegisterContainer>
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
                      color={currentColorScheme?.main?.accent}
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
                    {t("Common:Or")}
                  </Text>
                </div>
              )}

              {showForm && (
                <form className="auth-form-container">
                  <div className="auth-form-fields">
                    <FieldContainer
                      className="form-field"
                      isVertical={true}
                      labelVisible={false}
                      hasError={isEmailErrorShow && !emailValid}
                      errorMessage={
                        emailErrorText
                          ? t(`Common:${emailErrorText}`)
                          : t("Common:RequiredField")
                      }
                    >
                      <EmailInput
                        id="login"
                        name="login"
                        type="email"
                        hasError={isEmailErrorShow && !emailValid}
                        value={email}
                        placeholder={t("Common:Email")}
                        size="large"
                        scale={true}
                        isAutoFocussed={true}
                        tabIndex={1}
                        isDisabled={isLoading || !!emailFromLink}
                        autoComplete="username"
                        onChange={onChangeEmail}
                        onBlur={onBlurEmail}
                        onValidateInput={onValidateEmail}
                        forwardedRef={inputRef}
                        onKeyDown={onKeyPress}
                      />
                    </FieldContainer>

                    <FieldContainer
                      className="form-field"
                      isVertical={true}
                      labelVisible={false}
                      hasError={!fnameValid}
                      errorMessage={
                        errorText ? errorText : t("Common:RequiredField")
                      }
                    >
                      <TextInput
                        id="first-name"
                        name="first-name"
                        type="text"
                        hasError={!fnameValid}
                        value={fname}
                        placeholder={t("Common:FirstName")}
                        size="large"
                        scale={true}
                        tabIndex={1}
                        isDisabled={isLoading}
                        onChange={onChangeFname}
                        onKeyDown={onKeyPress}
                      />
                    </FieldContainer>

                    <FieldContainer
                      className="form-field"
                      isVertical={true}
                      labelVisible={false}
                      hasError={!snameValid}
                      errorMessage={
                        errorText ? errorText : t("Common:RequiredField")
                      }
                    >
                      <TextInput
                        id="last-name"
                        name="last-name"
                        type="text"
                        hasError={!snameValid}
                        value={sname}
                        placeholder={t("Common:LastName")}
                        size="large"
                        scale={true}
                        tabIndex={1}
                        isDisabled={isLoading}
                        onChange={onChangeSname}
                        onKeyDown={onKeyPress}
                      />
                    </FieldContainer>

                    <FieldContainer
                      className="form-field password-field"
                      isVertical={true}
                      labelVisible={false}
                      hasError={isPasswordErrorShow && !passwordValid}
                      errorMessage={`${t(
                        "Common:PasswordLimitMessage"
                      )}: ${getPasswordErrorMessage(t, settings)}`}
                    >
                      <PasswordInput
                        simpleView={false}
                        hideNewPasswordButton
                        showCopyLink={false}
                        passwordSettings={settings}
                        id="password"
                        inputName="password"
                        placeholder={t("Common:Password")}
                        type="password"
                        hasError={isPasswordErrorShow && !passwordValid}
                        inputValue={password}
                        size="large"
                        scale={true}
                        tabIndex={1}
                        isDisabled={isLoading}
                        autoComplete="current-password"
                        onChange={onChangePassword}
                        onBlur={onBlurPassword}
                        onKeyDown={onKeyPress}
                        onValidateInput={onValidatePassword}
                        tooltipPasswordTitle={`${t(
                          "Common:PasswordLimitMessage"
                        )}:`}
                        tooltipPasswordLength={`${t(
                          "Common:PasswordMinimumLength"
                        )}: ${settings ? settings.minLength : 8}`}
                        tooltipPasswordDigits={`${t(
                          "Common:PasswordLimitDigits"
                        )}`}
                        tooltipPasswordCapital={`${t(
                          "Common:PasswordLimitUpperCase"
                        )}`}
                        tooltipPasswordSpecial={`${t(
                          "Common:PasswordLimitSpecialSymbols"
                        )}`}
                        generatePasswordTitle={t("Wizard:GeneratePassword")}
                      />
                    </FieldContainer>

                    <Button
                      className="login-button"
                      primary
                      size="medium"
                      scale={true}
                      label={
                        isLoading
                          ? t("Common:LoadingProcessing")
                          : t("LoginRegistryButton")
                      }
                      tabIndex={1}
                      isDisabled={isLoading}
                      isLoading={isLoading}
                      onClick={onSubmit}
                    />
                  </div>
                </form>
              )}

              {!showForm && (
                <Button
                  className="login-button"
                  primary
                  size="medium"
                  scale={true}
                  label={
                    isLoading
                      ? t("Common:LoadingProcessing")
                      : t("LoginRegistryButton")
                  }
                  tabIndex={1}
                  isDisabled={isLoading}
                  isLoading={isLoading}
                  onClick={onGreetingJoin}
                />
              )}

              <MoreLoginModal
                t={t}
                visible={moreAuthVisible}
                onClose={moreAuthClose}
                providers={providers}
                onSocialLoginClick={onSocialButtonClick}
                ssoLabel={ssoLabel}
                ssoUrl={ssoUrl}
              />
            </RegisterContainer>
          </FormWrapper>
        </ConfirmContainer>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => {
  const {
    login,
    logout,
    isAuthenticated,
    settingsStore,
    providers,
    thirdPartyLogin,
    capabilities,
  } = auth;
  const {
    passwordSettings,
    greetingSettings,
    hashSettings,
    defaultPage,
    getSettings,
    getPortalPasswordSettings,
    currentColorScheme,
  } = settingsStore;

  return {
    settings: passwordSettings,
    greetingTitle: greetingSettings,
    hashSettings,
    defaultPage,
    isAuthenticated,
    login,
    logout,
    getSettings,
    getPortalPasswordSettings,
    thirdPartyLogin,
    providers,
    capabilities,
    currentColorScheme,
  };
})(
  withTranslation(["Confirm", "Common", "Wizard"])(
    withLoader(observer(CreateUserForm))
  )
);
