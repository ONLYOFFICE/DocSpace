import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import PasswordInput from "@appserver/components/password-input";
import Button from "@appserver/components/button";
import Section from "@appserver/common/components/Section";
import FieldContainer from "@appserver/components/field-container";
import { inject, observer } from "mobx-react";
import { StyledPage, StyledBody, StyledHeader } from "./StyledConfirm";
import withLoader from "../withLoader";
import { getPasswordErrorMessage } from "../../../../helpers/utils";
import { createPasswordHash } from "@appserver/common/utils";
import tryRedirectTo from "@appserver/common/utils/tryRedirectTo";
import toastr from "@appserver/components/toast/toastr";

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
        toastr.error(t(`${error}`));
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
          <Text fontSize="23px" fontWeight="700">
            {greetingTitle}
          </Text>
        </StyledHeader>

        <div className="password-change-form">
          <Text className="confirm-subtitle">{t("PassworResetTitle")}</Text>
          <FieldContainer
            className="form-field"
            isVertical={true}
            labelVisible={false}
            hasError={isPasswordErrorShow && !passwordValid}
            errorMessage={`${t(
              "Common:PasswordLimitMessage"
            )}: ${getPasswordErrorMessage(t, settings)}`}
          >
            <PasswordInput
              className="confirm-input"
              simpleView={false}
              passwordSettings={settings}
              id="password"
              inputName="password"
              placeholder={t("Common:Password")}
              type="password"
              inputValue={password}
              hasError={isPasswordErrorShow && !passwordValid}
              size="large"
              scale={true}
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
          className="confirm-button"
          primary
          size="normal"
          label={t("Common:Create")}
          tabIndex={5}
          onClick={onSubmit}
          isDisabled={isLoading}
        />
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
