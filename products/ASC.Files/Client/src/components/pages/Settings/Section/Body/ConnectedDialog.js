import React, { useState } from "react";
import styled from "styled-components";
import {
  ModalDialog,
  TextInput,
  Button,
  Checkbox,
  Text,
  utils,
} from "asc-web-components";

const { tablet } = utils.device;

const StyledConnectedDialog = styled.div`
  .dialog-form-container {
    display: flex;
    flex-direction: row;
    margin-bottom: 16px;

    @media ${tablet} {
      flex-direction: column;
      margin-bottom: 8px;
    }
  }
  .dialog-form-text {
    line-height: 32px;
    width: 160px;
    min-width: 110px;
    text-align: right;
    padding-right: 8px;

    @media ${tablet} {
      text-align: left;
      width: 100%;
    }
  }
`;

const ConnectedDialog = (props) => {
  const { onClose, visible, t, selectedService } = props;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [folderNameValue, setFolderNameValue] = useState("");
  const [makeShared, setMakeShared] = useState(false);

  const onChangeUrl = (e) => setUrlValue(e.target.value);
  const onChangeLogin = (e) => setLoginValue(e.target.value);
  const onChangePassword = (e) => setPasswordValue(e.target.value);
  const onChangeFolderName = (e) => setFolderNameValue(e.target.value);
  const onChangeMakeShared = (e) => setMakeShared(!makeShared);

  const onSave = () => {
    alert("onSave");
  };

  const showUrlField =
    selectedService === "WebDav" || selectedService === "SharePoint";

  return (
    <StyledConnectedDialog>
      <ModalDialog visible={visible} zIndex={310} onClose={onClose}>
        <ModalDialog.Header>{t("ConnectingAccount")}</ModalDialog.Header>
        <ModalDialog.Body>
          {showUrlField && (
            <div className="dialog-form-container">
              <Text className="dialog-form-text">{t("ConnectionUrl")}</Text>
              <TextInput scale value={urlValue} onChange={onChangeUrl} />
            </div>
          )}

          <div className="dialog-form-container">
            <Text className="dialog-form-text">{t("Login")}</Text>
            <TextInput scale value={loginValue} onChange={onChangeLogin} />
          </div>
          <div className="dialog-form-container">
            <Text className="dialog-form-text">{t("Password")}</Text>
            <TextInput
              scale
              value={passwordValue}
              onChange={onChangePassword}
            />
          </div>
          <div className="dialog-form-container">
            <Text className="dialog-form-text">{t("ConnectFolderTitle")}</Text>
            <TextInput
              scale
              value={folderNameValue}
              onChange={onChangeFolderName}
            />
          </div>
          <div className="dialog-form-container">
            <Text className="dialog-form-text" />
            <Checkbox
              label={t("ConnectMakeShared")}
              isChecked={makeShared}
              onChange={onChangeMakeShared}
            />
          </div>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button label={t("SaveButton")} size="big" primary onClick={onSave} />
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledConnectedDialog>
  );
};

export default ConnectedDialog;
