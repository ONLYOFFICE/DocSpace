import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import PasswordInput from "@docspace/components/password-input";
import Button from "@docspace/components/button";
import Section from "@docspace/common/components/Section";
import FieldContainer from "@docspace/components/field-container";
import { inject, observer } from "mobx-react";
import { StyledPage, StyledBody, StyledHeader } from "./StyledConfirm";
import withLoader from "../withLoader";
import { getPasswordErrorMessage } from "../../../helpers/utils";
import { createPasswordHash } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";
import toastr from "@docspace/components/toast/toastr";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

const ChangePasswordForm = (props) => {
  const {
    t,
    greetingTitle,
    settings,
    hashSettings,
    defaultPage,
    logout,
    changePassword,
    linkData,
  } = props;

  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [isPasswordErrorShow, setIsPasswordErrorShow] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  const onValidatePassword = (res) => {
    setPasswordValid(res);
  };

  const onBlurPassword = () => {
    setIsPasswordErrorShow(true);
  };

  const onSubmit = () => {
    setIsLoading(true);

    if (!password.trim()) {
      setPasswordValid(false);
      setIsPasswordErrorShow(true);
    }
    if (!passwordValid || !password.trim()) {
      setIsLoading(false);
      return;
    }

    const hash = createPasswordHash(password, hashSettings);
    const { uid, confirmHeader } = linkData;
    changePassword(uid, hash, confirmHeader)
      .then(() => logout())
      .then(() => {
        setIsLoading(false);
        toastr.success(t("ChangePasswordSuccess"));
        tryRedirectTo(defaultPage);
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

        toastr.error(t(`${errorMessage}`));
        setIsLoading(false);
      });
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSubmit();
    }
  };

  return (
    <StyledPage>
      <StyledBody>
        <StyledHeader>
          <DocspaceLogo className="docspace-logo" />
          <Text fontSize="23px" fontWeight="700" className="title">
            {greetingTitle}
          </Text>
        </StyledHeader>

        <FormWrapper>
          <div className="password-form">
            <Text fontSize="16px" fontWeight="600" className="subtitle">
              {t("PassworResetTitle")}
            </Text>
            <FieldContainer
              isVertical={true}
              labelVisible={false}
              hasError={isPasswordErrorShow && !passwordValid}
              errorMessage={`${t(
                "Common:PasswordLimitMessage"
              )}: ${getPasswordErrorMessage(t, settings)}`}
            >
              <PasswordInput
                simpleView={false}
                passwordSettings={settings}
                id="password"
                inputName="password"
                placeholder={t("Common:Password")}
                type="password"
                inputValue={password}
                hasError={isPasswordErrorShow && !passwordValid}
                size="large"
                scale
                tabIndex={1}
                autoComplete="current-password"
                onChange={onChangePassword}
                onValidateInput={onValidatePassword}
                onBlur={onBlurPassword}
                onKeyDown={onKeyPress}
                tooltipPasswordTitle={`${t("Common:PasswordLimitMessage")}:`}
                tooltipPasswordLength={`${t("Common:PasswordMinimumLength")}: ${
                  settings ? settings.minLength : 8
                }`}
                tooltipPasswordDigits={`${t("Common:PasswordLimitDigits")}`}
                tooltipPasswordCapital={`${t("Common:PasswordLimitUpperCase")}`}
                tooltipPasswordSpecial={`${t(
                  "Common:PasswordLimitSpecialSymbols"
                )}`}
                generatePasswordTitle={t("Wizard:GeneratePassword")}
              />
            </FieldContainer>
          </div>

          <Button
            primary
            size="medium"
            scale
            label={t("Common:Create")}
            tabIndex={5}
            onClick={onSubmit}
            isDisabled={isLoading}
          />
        </FormWrapper>
      </StyledBody>
    </StyledPage>
  );
};

const ChangePasswordFormWrapper = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <ChangePasswordForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth, setup }) => {
  const {
    greetingSettings,
    hashSettings,
    defaultPage,
    passwordSettings,
    theme,
  } = auth.settingsStore;
  const { changePassword } = setup;

  return {
    theme,
    settings: passwordSettings,
    greetingTitle: greetingSettings,
    hashSettings,
    defaultPage,
    logout: auth.logout,
    changePassword,
    isAuthenticated: auth.isAuthenticated,
  };
})(
  withRouter(
    withTranslation(["Confirm", "Common", "Wizard"])(
      withLoader(observer(ChangePasswordFormWrapper))
    )
  )
);
