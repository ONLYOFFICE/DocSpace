import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import TextInput from "@docspace/components/text-input";
import PasswordInput from "@docspace/components/password-input";
import Button from "@docspace/components/button";
import FieldContainer from "@docspace/components/field-container";
import { inject, observer } from "mobx-react";
import { EmployeeActivationStatus } from "@docspace/common/constants";
import {
  changePassword,
  updateActivationStatus,
  updateUser,
} from "@docspace/common/api/people";
import { createPasswordHash } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";
import {
  StyledPage,
  StyledContent,
  StyledBody,
  StyledHeader,
} from "./StyledConfirm";
import withLoader from "../withLoader";
import { getPasswordErrorMessage } from "SRC_DIR/helpers/utils";

const ActivateUserForm = (props) => {
  const {
    t,
    greetingTitle,
    settings,
    linkData,
    hashSettings,
    defaultPage,
    login,
  } = props;

  const [name, setName] = useState(linkData.firstname);
  const [nameValid, setNameValid] = useState(true);
  const [surName, setSurName] = useState(linkData.lastname);
  const [surNameValid, setSurNameValid] = useState(true);
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [isPasswordErrorShow, setIsPasswordErrorShow] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const onChangeName = (e) => {
    setName(e.target.value);
    setNameValid(true);
  };

  const onChangeSurName = (e) => {
    setSurName(e.target.value);
    setSurNameValid(true);
  };

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
    if (!name.trim()) setNameValid(false);
    if (!surName.trim()) setSurNameValid(false);
    if (!password.trim()) {
      setPasswordValid(false);
      setIsPasswordErrorShow(true);
    }

    if (!nameValid || !surNameValid || !password.trim() || !passwordValid) {
      setIsLoading(false);
      return;
    }

    const hash = createPasswordHash(password, hashSettings);

    const loginData = {
      userName: linkData.email,
      passwordHash: hash,
    };

    const personalData = {
      firstname: name,
      lastname: surName,
    };

    activateConfirmUser(
      personalData,
      loginData,
      linkData.confirmHeader,
      linkData.uid,
      EmployeeActivationStatus.Activated
    )
      .then(() => {
        setIsLoading(false);
        window.location.replace(defaultPage);
      })
      .catch((error) => {
        //console.error(error);
        setIsLoading(false);
        toastr.error(error);
      });
  };

  const activateConfirmUser = async (
    personalData,
    loginData,
    key,
    userId,
    activationStatus
  ) => {
    const changedData = {
      id: userId,
      FirstName: personalData.firstname,
      LastName: personalData.lastname,
    };

    const { userName, passwordHash } = loginData;

    const res1 = await changePassword(userId, loginData.passwordHash, key);
    const res2 = await updateActivationStatus(activationStatus, userId, key);
    const res3 = await login(userName, passwordHash);
    const res4 = await updateUser(changedData);
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSubmit();
    }
  };

  return (
    <StyledPage>
      <StyledContent>
        <StyledBody>
          <StyledHeader>
            <Text fontSize="23px" fontWeight="700" className="title">
              {greetingTitle}
            </Text>

            <Text className="subtitle">{t("InviteTitle")}</Text>
          </StyledHeader>

          <FieldContainer
            className="form-field"
            isVertical={true}
            labelVisible={false}
            hasError={!nameValid}
            errorMessage={t("Common:RequiredField")}
          >
            <TextInput
              id="name"
              name="name"
              value={name}
              placeholder={t("Common:FirstName")}
              size="large"
              scale={true}
              tabIndex={1}
              isAutoFocussed={true}
              autoComplete="given-name"
              onChange={onChangeName}
              onKeyDown={onKeyPress}
            />
          </FieldContainer>

          <FieldContainer
            className="form-field"
            isVertical={true}
            labelVisible={false}
            hasError={!surNameValid}
            errorMessage={t("Common:RequiredField")}
          >
            <TextInput
              id="surname"
              name="surname"
              value={surName}
              placeholder={t("Common:LastName")}
              size="large"
              scale={true}
              tabIndex={2}
              autoComplete="family-name"
              onChange={onChangeSurName}
              onKeyDown={onKeyPress}
            />
          </FieldContainer>

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
              // If need copy credentials use t("EmailAndPasswordCopiedToClipboard")
            />
          </FieldContainer>

          <Button
            className="confirm-button"
            primary
            size="normal"
            label={t("LoginRegistryButton")}
            tabIndex={5}
            onClick={onSubmit}
            isDisabled={isLoading}
          />
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth }) => {
  const {
    greetingSettings,
    hashSettings,
    defaultPage,
    passwordSettings,
    theme,
  } = auth.settingsStore;

  return {
    theme,
    settings: passwordSettings,
    greetingTitle: greetingSettings,
    hashSettings,
    defaultPage,
    login: auth.login,
  };
})(
  withRouter(
    withTranslation(["Confirm", "Common", "Wizard"])(
      withLoader(observer(ActivateUserForm))
    )
  )
);
