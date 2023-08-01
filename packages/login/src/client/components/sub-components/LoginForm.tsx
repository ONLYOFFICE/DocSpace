import React, { useState, useRef, useEffect } from "react";
import FieldContainer from "@docspace/components/field-container";
import EmailInput from "@docspace/components/email-input";
import PasswordInput from "@docspace/components/password-input";
import Checkbox from "@docspace/components/checkbox";
import HelpButton from "@docspace/components/help-button";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { useTranslation } from "react-i18next";
import ForgotPasswordModalDialog from "./forgot-password-modal-dialog";
import Button from "@docspace/components/button";
import { checkIsSSR, createPasswordHash } from "@docspace/common/utils";
import { checkPwd } from "@docspace/common/desktop";
import { login } from "@docspace/common/utils/loginUtils";
import toastr from "@docspace/components/toast/toastr";
import { thirdPartyLogin } from "@docspace/common/api/user";
import { setWithCredentialsStatus } from "@docspace/common/api/client";
import { isMobileOnly } from "react-device-detect";

interface ILoginFormProps {
  isLoading: boolean;
  setIsLoading: (isLoading: boolean) => void;
  hashSettings: PasswordHashType;
  isDesktop: boolean;
  match: MatchType;
  onRecoverDialogVisible: () => void;
  enableAdmMess: boolean;
}

const settings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false,
};

const LoginForm: React.FC<ILoginFormProps> = ({
  isLoading,
  hashSettings,
  isDesktop,
  match,
  setIsLoading,
  onRecoverDialogVisible,
  enableAdmMess,
  cookieSettingsEnabled,
}) => {
  const [isEmailErrorShow, setIsEmailErrorShow] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [identifier, setIdentifier] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [identifierValid, setIdentifierValid] = useState(true);
  const [password, setPassword] = useState("");
  const [isDisabled, setIsDisabled] = useState(false);
  const [isChecked, setIsChecked] = useState(false);
  const [isDialogVisible, setIsDialogVisible] = useState(false);
  const [isWithoutPasswordLogin, setIsWithoutPasswordLogin] = useState(
    IS_ROOMS_MODE
  );

  const inputRef = useRef<HTMLInputElement>(null);

  const { t, ready } = useTranslation(["Login", "Common"]);

  const { message, confirmedEmail, authError } = match || {
    message: "",
    confirmedEmail: "",
    authError: "",
  };

  const authCallback = (profile: string) => {
    localStorage.removeItem("profile");
    localStorage.removeItem("code");

    thirdPartyLogin(profile)
      .then((response) => {
        if (!response || !response.token) throw new Error("Empty API response");

        setWithCredentialsStatus(true);
        const redirectPath = sessionStorage.getItem("referenceUrl");

        if (redirectPath) {
          sessionStorage.removeItem("referenceUrl");
          window.location.href = redirectPath;
        } else {
          window.location.replace("/");
        }
      })
      .catch(() => {
        toastr.error(
          t("Common:ProviderNotConnected"),
          t("Common:ProviderLoginError")
        );
      });
  };

  useEffect(() => {
    const profile = localStorage.getItem("profile");
    if (!profile) return;

    authCallback(profile);
  }, []);

  useEffect(() => {
    message && setErrorText(message);
    confirmedEmail && setIdentifier(confirmedEmail);

    const messageEmailConfirmed = t("MessageEmailConfirmed");
    const messageAuthorize = t("MessageAuthorize");

    const text = `${messageEmailConfirmed} ${messageAuthorize}`;

    confirmedEmail && ready && toastr.success(text);
    authError && ready && toastr.error(t("Common:ProviderLoginError"));

    focusInput();

    window.authCallback = authCallback;
  }, [message, confirmedEmail]);

  const onChangeLogin = (e: React.ChangeEvent<HTMLInputElement>) => {
    //console.log("onChangeLogin", e.target.value);
    setIdentifier(e.target.value);
    if (!IS_ROOMS_MODE) setIsEmailErrorShow(false);
    onClearErrors();
  };

  const onClearErrors = () => {
    if (IS_ROOMS_MODE) {
      !identifierValid && setIdentifierValid(true);
      errorText && setErrorText("");
      setIsEmailErrorShow(false);
    } else {
      !passwordValid && setPasswordValid(true);
    }
  };

  const onSubmit = () => {
    //errorText && setErrorText("");
    let hasError = false;

    const user = identifier.trim();

    if (!user) {
      hasError = true;
      setIdentifierValid(false);
      setIsEmailErrorShow(true);
    }

    if (IS_ROOMS_MODE && identifierValid) {
      window.location.replace("/login/code"); //TODO: confirm link?
      return;
    }

    const pass = password.trim();

    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (!identifierValid) hasError = true;

    if (hasError) return;

    setIsLoading(true);
    const hash = createPasswordHash(pass, hashSettings);

    isDesktop && checkPwd();
    const session = !isChecked;
    login(user, hash, session)
      .then((res: string | object) => {
        const isConfirm = typeof res === "string" && res.includes("confirm");
        const redirectPath = sessionStorage.getItem("referenceUrl");
        if (redirectPath && !isConfirm) {
          sessionStorage.removeItem("referenceUrl");
          window.location.href = redirectPath;
          return;
        }

        if (typeof res === "string") window.location.replace(res);
        else window.location.replace("/"); //TODO: save { user, hash } for tfa
      })
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

        setIsEmailErrorShow(true);
        setErrorText(errorMessage);
        setPasswordValid(!errorMessage);
        setIsLoading(false);
        focusInput();
      });
  };

  const onLoginWithPasswordClick = () => {
    setIsWithoutPasswordLogin(false);
  };

  const onBlurEmail = () => {
    !identifierValid && setIsEmailErrorShow(true);
  };

  const onValidateEmail = (res: IEmailValid) => {
    setIdentifierValid(res.isValid);
    setErrorText(res.errors[0]);
  };

  const focusInput = () => {
    if (inputRef && inputRef.current) {
      inputRef.current.focus();
    }
  };

  const onChangePassword = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(e.target.value);
    onClearErrors();
  };

  const onKeyDown = (e: KeyboardEvent) => {
    if (e.key === "Enter") {
      onClearErrors();
      !isDisabled && onSubmit();
      e.preventDefault();
    }
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

  return (
    <form className="auth-form-container">
      <FieldContainer
        isVertical={true}
        labelVisible={false}
        hasError={isEmailErrorShow}
        errorMessage={
          errorText ? t(`Common:${errorText}`) : t("Common:RequiredField")
        } //TODO: Add wrong login server error
      >
        <EmailInput
          id="login_username"
          name="login"
          type="email"
          hasError={isEmailErrorShow}
          value={identifier}
          placeholder={t("RegistrationEmailWatermark")}
          size="large"
          scale={true}
          isAutoFocussed={true}
          tabIndex={1}
          isDisabled={isLoading}
          autoComplete="username"
          onChange={onChangeLogin}
          onBlur={onBlurEmail}
          onValidateInput={onValidateEmail}
          forwardedRef={inputRef}
        />
      </FieldContainer>
      {(!IS_ROOMS_MODE || !isWithoutPasswordLogin) && (
        <>
          <FieldContainer
            isVertical={true}
            labelVisible={false}
            hasError={!passwordValid}
            errorMessage={!password.trim() ? t("Common:RequiredField") : ""} //TODO: Add wrong password server error
          >
            <PasswordInput
              className="password-input"
              simpleView={true}
              passwordSettings={settings}
              id="login_password"
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
              <div className="remember-wrapper">
                {!cookieSettingsEnabled && (
                  <Checkbox
                    id="login_remember"
                    className="login-checkbox"
                    isChecked={isChecked}
                    onChange={onChangeCheckbox}
                    label={t("Remember")}
                    helpButton={
                      !checkIsSSR() && (
                        <HelpButton
                          id="login_remember-hint"
                          className="help-button"
                          offsetRight={0}
                          helpButtonHeaderContent={t("CookieSettingsTitle")}
                          tooltipContent={
                            <Text fontSize="12px">{t("RememberHelper")}</Text>
                          }
                          tooltipMaxWidth={isMobileOnly ? "240px" : "340px"}
                        />
                      )
                    }
                  />
                )}
              </div>

              <Link
                fontSize="13px"
                className="login-link"
                type="page"
                isHovered={false}
                onClick={onClick}
                id="login_forgot-password-link"
              >
                {t("ForgotPassword")}
              </Link>
            </div>
          </div>

          {isDialogVisible && (
            <ForgotPasswordModalDialog
              isVisible={isDialogVisible}
              userEmail={identifier}
              onDialogClose={onDialogClose}
            />
          )}
        </>
      )}

      <Button
        id="login_submit"
        className="login-button"
        primary
        size="medium"
        scale={true}
        label={
          isLoading ? t("Common:LoadingProcessing") : t("Common:LoginButton")
        }
        tabIndex={1}
        isDisabled={isLoading}
        isLoading={isLoading}
        onClick={onSubmit}
      />
      {/*Uncomment when add api*/}
      {(!IS_ROOMS_MODE || !isWithoutPasswordLogin) && (
        <div className="login-or-access">
          {/*<Link
                  fontWeight="600"
                  fontSize="13px"
                  type="action"
                  isHovered={true}
                  onClick={onLoginWithCodeClick}
                >
                  {t("SignInWithCode")}
                </Link>*/}
          {enableAdmMess && (
            <>
              <Text className="login-or-access-text">{t("Or")}</Text>
              <Link
                id="login_recover-link"
                fontWeight="600"
                fontSize="13px"
                type="action"
                isHovered={true}
                className="login-link recover-link"
                onClick={onRecoverDialogVisible}
              >
                {t("RecoverAccess")}
              </Link>
            </>
          )}
        </div>
      )}

      {IS_ROOMS_MODE && isWithoutPasswordLogin && (
        <div className="login-link">
          <Link
            fontWeight="600"
            fontSize="13px"
            type="action"
            isHovered={true}
            onClick={onLoginWithPasswordClick}
          >
            {t("SignInWithPassword")}
          </Link>
        </div>
      )}
    </form>
  );
};

export default LoginForm;
