import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import PasswordInput from "@docspace/components/password-input";
import Button from "@docspace/components/button";
import FieldContainer from "@docspace/components/field-container";
import { inject, observer } from "mobx-react";
import { StyledPage, StyledBody, StyledContent } from "./RoomStyles";
// import withLoader from "../withLoader";
import { getPasswordErrorMessage } from "../../../helpers/utils";
import { createPasswordHash } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";
import toastr from "@docspace/components/toast/toastr";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";

import PublicRoomIcon from "PUBLIC_DIR/images/icons/32/room/public.svg";

const RoomPassword = (props) => {
  const {
    t,
    settings,
    hashSettings,
    defaultPage,
    logout,
    changePassword,
    linkData,
    getSettings,
  } = props;

  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [isPasswordErrorShow, setIsPasswordErrorShow] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!hashSettings) getSettings(true);
  }, []);

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  const onValidatePassword = (res) => {
    setPasswordValid(res);
  };

  const onBlurPassword = () => {
    setIsPasswordErrorShow(true);
  };

  const onSubmit = async () => {
    setIsLoading(true);

    if (!password.trim()) {
      setPasswordValid(false);
      setIsPasswordErrorShow(true);
    }
    if (!passwordValid || !password.trim()) {
      setIsLoading(false);
      return;
    }

    try {
      const hash = createPasswordHash(password, hashSettings);
      const { uid, confirmHeader } = linkData;
      await changePassword(uid, hash, confirmHeader);
      logout();
      setIsLoading(false);
      toastr.success(t("ChangePasswordSuccess"));
      tryRedirectTo(defaultPage);
    } catch (error) {
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
      console.error(errorMessage);

      if (errorMessage === "Invalid params") {
        toastr.error(t("Common:SomethingWentWrong"));
      } else {
        toastr.error(t(`${errorMessage}`));
      }
      setIsLoading(false);
    }
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSubmit();
    }
  };

  const EnterPassword = "Enter password";
  const NeedPassword = "You need a password to access the room";
  const roomName = "Certifications";

  return (
    <StyledPage>
      <StyledContent className="public-room-content">
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />

          <FormWrapper>
            <div className="password-form">
              <Text fontSize="16px" fontWeight="600">
                {EnterPassword}
              </Text>

              <Text
                fontSize="13px"
                fontWeight="400"
                className="public-room-text"
              >
                {NeedPassword}:
              </Text>
              <div className="public-room-name">
                <PublicRoomIcon />
                <Text fontSize="15px" fontWeight="600">
                  {roomName}
                </Text>
              </div>

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
                  tooltipPasswordLength={`${t(
                    "Common:PasswordMinimumLength"
                  )}: ${settings ? settings.minLength : 8}`}
                  tooltipPasswordDigits={`${t("Common:PasswordLimitDigits")}`}
                  tooltipPasswordCapital={`${t(
                    "Common:PasswordLimitUpperCase"
                  )}`}
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
              label={t("Common:ContinueButton")}
              tabIndex={5}
              onClick={onSubmit}
              isDisabled={isLoading}
            />
          </FormWrapper>
        </StyledBody>
      </StyledContent>
    </StyledPage>
  );
};

export default inject(({ auth, setup }) => {
  const {
    hashSettings,
    defaultPage,
    passwordSettings,
    theme,
    getSettings,
  } = auth.settingsStore;
  const { changePassword } = setup;

  return {
    theme,
    settings: passwordSettings,
    hashSettings,
    defaultPage,
    logout: auth.logout,
    changePassword,
    isAuthenticated: auth.isAuthenticated,
    getSettings,
  };
})(
  withTranslation(["Confirm", "Common", "Wizard"])(
    /* withLoader( */ observer(RoomPassword) /* ) */
  )
);
