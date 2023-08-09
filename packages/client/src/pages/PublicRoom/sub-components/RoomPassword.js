import React, { useState } from "react";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import PasswordInput from "@docspace/components/password-input";
import Button from "@docspace/components/button";
import FieldContainer from "@docspace/components/field-container";
import { inject, observer } from "mobx-react";
import { StyledPage, StyledBody, StyledContent } from "./RoomStyles";
// import { createPasswordHash } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";
import FormWrapper from "@docspace/components/form-wrapper";
import DocspaceLogo from "../../../DocspaceLogo";
import { ValidationResult } from "../../../helpers/constants";

import PublicRoomIcon from "PUBLIC_DIR/images/icons/32/room/public.svg";

const RoomPassword = (props) => {
  const { t, roomKey, validatePublicRoomPassword, setRoomData, roomTitle } =
    props;

  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);

  const onChangePassword = (e) => {
    setPassword(e.target.value);
    !passwordValid && setPasswordValid(true);
  };

  const onSubmit = async () => {
    if (!password.trim()) {
      setPasswordValid(false);
      setErrorMessage(t("Common:RequiredField"));
    }

    if (!passwordValid || !password.trim()) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    try {
      const res = await validatePublicRoomPassword(roomKey, password);

      setIsLoading(false);

      switch (res?.status) {
        case ValidationResult.Ok: {
          setRoomData(res); // Ok
          return;
        }
        // case ValidationResult.Invalid: {
        //   setErrorMessage(""); // Invalid
        //   toastr.error("Invalid");
        //   return;
        // }
        // case ValidationResult.Expired: {
        //   setErrorMessage(""); // Expired
        //   toastr.error("Expired");
        //   return;
        // }
        case ValidationResult.InvalidPassword: {
          setErrorMessage(t("Common:IncorrectPassword"));
          return;
        }
      }
    } catch (error) {
      console.log(error);
      setIsLoading(false);
    }
  };

  const onKeyPress = (event) => {
    if (event.key === "Enter") {
      onSubmit();
    }
  };

  return (
    <StyledPage>
      <StyledContent className="public-room-content">
        <StyledBody>
          <DocspaceLogo className="docspace-logo" />

          <FormWrapper>
            <div className="password-form">
              <Text fontSize="16px" fontWeight="600">
                {t("UploadPanel:EnterPassword")}
              </Text>

              <Text
                fontSize="13px"
                fontWeight="400"
                className="public-room-text"
              >
                {t("Common:NeedPassword")}:
              </Text>
              <div className="public-room-name">
                <PublicRoomIcon className="public-room-icon" />
                <Text
                  className="public-room-text"
                  fontSize="15px"
                  fontWeight="600"
                >
                  {roomTitle}
                </Text>
              </div>

              <FieldContainer
                isVertical={true}
                labelVisible={false}
                hasError={!!errorMessage}
                errorMessage={errorMessage}
              >
                <PasswordInput
                  simpleView
                  id="password"
                  inputName="password"
                  placeholder={t("Common:Password")}
                  type="password"
                  inputValue={password}
                  hasError={!!errorMessage}
                  size="large"
                  scale
                  tabIndex={1}
                  autoComplete="current-password"
                  onChange={onChangePassword}
                  onKeyDown={onKeyPress}
                  isDisabled={isLoading}
                  isDisableTooltip
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

export default inject(({ publicRoomStore }) => {
  const { validatePublicRoomPassword, setRoomData } = publicRoomStore;
  const { roomTitle } = publicRoomStore;

  return {
    validatePublicRoomPassword,
    setRoomData,
    roomTitle,
  };
})(withTranslation(["Common", "UploadPanel"])(observer(RoomPassword)));
