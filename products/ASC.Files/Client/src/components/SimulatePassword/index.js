import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import styled from "styled-components";
import InputBlock from "@appserver/components/input-block";
import globalColors from "@appserver/components/utils/globalColors";

const iconColor = globalColors.gray;
const bulletsFont = "•";

const StyledBody = styled.div`
  display: flex;
  width: 470px;

  #conversion-button {
    margin-left: 8px;
    background-color: #a6dcf2;
    width: 100%;
    max-width: 78px;
  }
  #conversion-password {
    max-width: 382px;
    width: 100%;
    margin: 0;
  }
  .conversion-input {
    width: 100%;
  }
`;
const SimulatePassword = ({ onClickActions }) => {
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

  const [caretPosition, setCaretPosition] = useState();
  const [inputType, setInputType] = useState("password");
  const { t } = useTranslation("UploadPanel");
  const onClick = () => {
    let hasError = false;

    const pass = password.trim();
    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (hasError) return;

    onClickActions(password);
  };

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
    caretPosition &&
      inputType === "password" &&
      document
        .getElementById("conversion-password")
        .setSelectionRange(caretPosition, caretPosition);
  }, [password]);

  return (
    <StyledBody className="conversation-password-wrapper">
      <>
        <InputBlock
          id="conversion-password"
          className="conversion-input"
          type="text"
          hasError={!passwordValid}
          iconName={iconName}
          value={inputType === "password" ? bullets : password}
          onIconClick={onChangeInputType}
          onChange={onChangePassword}
          scale
          iconSize={16}
          iconColor={iconColor}
          hoverColor={iconColor}
          placeholder={t("EnterPassword")}
        ></InputBlock>

        <Button
          id="conversion-button"
          size="medium"
          scale
          primary
          label={t("Done")}
          onClick={onClick}
        />
      </>
    </StyledBody>
  );
};

export default SimulatePassword;
