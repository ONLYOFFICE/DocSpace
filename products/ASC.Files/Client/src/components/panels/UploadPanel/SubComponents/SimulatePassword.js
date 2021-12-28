import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import InputBlock from "@appserver/components/input-block";
import globalColors from "@appserver/components/utils/globalColors";

const iconColor = globalColors.gray;

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
const SimulatePassword = ({
  item,
  convertFile,
  removeFileFromList,
  hideInput,
  uploadedFiles,
}) => {
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

  const [cursorPosition, setCursorPosition] = useState();
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
    let index;

    uploadedFiles.reduce((acc, rec, id) => {
      if (rec.fileId === item.fileId) index = id;
    }, []);

    const newItem = {
      fileId: item.fileId,
      toFolderId: item.toFolderId,
      action: "convert",
      fileInfo: item.fileInfo,
      password: password,
      index,
    };

    hideInput();
    removeFileFromList(item.fileId);
    convertFile(newItem);
  };

  const setPasswordSettings = (newPassword) => {
    const cursorPosition = document.getElementById("conversion-password")
      .selectionStart;

    setCursorPosition(cursorPosition);

    const copyPassword = password;
    const passwordLength = password.length;
    const newPasswordLength = newPassword.length;

    if (passwordLength < newPasswordLength) {
      if (cursorPosition === newPasswordLength) {
        setPassword(password + newPassword.slice(-1));
      } else {
        const passwordConversion = copyPassword.split("");
        const newPasswordConversion = newPassword.split("");
        const newSymbol = newPasswordConversion[cursorPosition - 1];

        passwordConversion.splice(cursorPosition - 1, 0, newSymbol);

        setPassword(passwordConversion.join(""));
      }
    } else {
      const itemsCountRemoved = passwordLength - newPasswordLength;
      const passwordConversion = copyPassword.split("");

      passwordConversion.splice(cursorPosition, itemsCountRemoved);

      setPassword(passwordConversion.join(""));
    }
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
  const bullets = copyPassword.replace(/(.)/g, "•");

  const iconName =
    inputType === "password"
      ? "/static/images/eye.off.react.svg"
      : "/static/images/eye.react.svg";

  useEffect(() => {
    cursorPosition &&
      inputType === "password" &&
      document
        .getElementById("conversion-password")
        .setSelectionRange(cursorPosition, cursorPosition);
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

export default inject(({ uploadDataStore }) => {
  const {
    convertFile,
    removeFileFromList,
    files: uploadedFiles,
  } = uploadDataStore;

  return {
    uploadedFiles,
    removeFileFromList,
    convertFile,
  };
})(observer(SimulatePassword));
