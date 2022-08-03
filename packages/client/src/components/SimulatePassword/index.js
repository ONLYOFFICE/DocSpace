import React, { useState, useEffect, memo } from "react";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";
import InputBlock from "@docspace/components/input-block";
import globalColors from "@docspace/components/utils/globalColors";

const iconColor = globalColors.gray;

const bulletsFont = "•";

const StyledBody = styled.div`
  width: 100%;

  #conversion-password {
    max-width: ${(props) =>
      props.inputMaxWidth ? props.inputMaxWidth : "382px"};
    width: 100%;
    margin: 0;
  }
  .conversion-input {
    width: 100%;
  }
`;
const SimulatePassword = memo(
  ({
    onChange,
    onKeyDown,
    inputMaxWidth,
    isDisabled = false,
    hasError = false,
    forwardedRef,
  }) => {
    const [password, setPassword] = useState("");
    const [caretPosition, setCaretPosition] = useState();
    const [inputType, setInputType] = useState("password");

    const { t } = useTranslation("UploadPanel");

    const setPasswordSettings = (newPassword) => {
      let newValue;

      const oldPassword = password;
      const oldPasswordLength = oldPassword.length;
      const caretPosition = document.getElementById("conversion-password")
        .selectionStart;

      setCaretPosition(caretPosition);
      const newCharactersUntilCaret = newPassword.substring(0, caretPosition);

      const unchangedStartCharacters = newCharactersUntilCaret
        .split("")
        .filter((el) => el === bulletsFont).length;

      const unchangedEndingCharacters = newPassword.substring(caretPosition)
        .length;
      const addedCharacters = newCharactersUntilCaret.substring(
        unchangedStartCharacters
      );

      const startingPartOldPassword = oldPassword.substring(
        0,
        unchangedStartCharacters
      );
      const countOfCharacters = oldPasswordLength - unchangedEndingCharacters;
      const endingPartOldPassword = oldPassword.substring(countOfCharacters);

      newValue = startingPartOldPassword + addedCharacters;

      if (unchangedEndingCharacters) {
        newValue += endingPartOldPassword;
      }

      setPassword(newValue);
    };

    const onChangePassword = (e) => {
      const newPassword = e.target.value;

      inputType == "password"
        ? setPasswordSettings(newPassword)
        : setPassword(newPassword);
    };

    const onKeyDownAction = (e) => {
      if (e.key === "Enter") {
        onKeyDown && onKeyDown(e);
      }
    };

    const onChangeInputType = () => {
      setInputType(inputType === "password" ? "text" : "password");
    };

    const copyPassword = password;
    const bullets = copyPassword.replace(/(.)/g, bulletsFont);

    const iconName =
      inputType === "password"
        ? "/static/images/eye.off.react.svg"
        : "/static/images/eye.react.svg";

    useEffect(() => {
      onChange && onChange(password);

      caretPosition &&
        inputType === "password" &&
        document
          .getElementById("conversion-password")
          .setSelectionRange(caretPosition, caretPosition);
    }, [password]);

    useEffect(() => {
      isDisabled && inputType !== "password" && setInputType("password");
    }, [isDisabled]);

    return (
      <StyledBody
        className="conversation-password-wrapper"
        inputMaxWidth={inputMaxWidth}
      >
        <InputBlock
          id="conversion-password"
          className="conversion-input"
          type="text"
          hasError={hasError}
          isDisabled={isDisabled}
          iconName={iconName}
          value={inputType === "password" ? bullets : password}
          onIconClick={onChangeInputType}
          onChange={onChangePassword}
          onKeyDown={onKeyDownAction}
          scale
          iconSize={16}
          iconColor={iconColor}
          hoverColor={iconColor}
          placeholder={t("EnterPassword")}
          forwardedRef={forwardedRef}
          isAutoFocussed
        />
      </StyledBody>
    );
  }
);

SimulatePassword.propTypes = {
  inputMaxWidth: PropTypes.string,
  hasError: PropTypes.bool,
  onChange: PropTypes.func,
};
export default SimulatePassword;
