import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import PasswordInput from "@appserver/components/password-input";
import Button from "@appserver/components/button";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

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
const PasswordComponent = ({
  item,
  convertFile,
  removeFileFromList,
  hideInput,
  uploadedFiles,
}) => {
  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

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

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  return (
    <StyledBody className="conversation-password-wrapper">
      <PasswordInput
        simpleView
        id="conversion-password"
        className="conversion-input"
        type="password"
        inputValue={password}
        onChange={onChangePassword}
        placeholder={t("EnterPassword")}
        hasError={!passwordValid}
      />
      <Button
        id="conversion-button"
        size="medium"
        scale
        primary
        label={t("Ready")}
        onClick={onClick}
      />
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
})(observer(PasswordComponent));
