import React, { useState } from "react";
import styled from "styled-components";
import {
  ModalDialog,
  TextInput,
  PasswordInput,
  Button,
  Checkbox,
  Text,
  utils,
} from "asc-web-components";
import {
  saveThirdParty,
  openConnectWindow,
} from "../../../../../store/files/actions";

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
    .dialog-form-input {
      width: 100%;
    }
  }
  .dialog-form-text {
    line-height: 32px;
    min-width: 160px;
    text-align: right;
    padding-right: 8px;

    @media ${tablet} {
      text-align: left;
      width: 100%;
    }
  }
`;

const ConnectedDialog = (props) => {
  const { onClose, visible, t, item } = props;
  const { corporate, title, link, auth_key, provider_id, provider_key } = item;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [customerTitle, setCustomerTitleValue] = useState(title);
  const [isCorporate, setMakeShared] = useState(!!corporate);

  const onChangeUrl = (e) => setUrlValue(e.target.value);
  const onChangeLogin = (e) => setLoginValue(e.target.value);
  const onChangePassword = (e) => setPasswordValue(e.target.value);
  const onChangeFolderName = (e) => setCustomerTitleValue(e.target.value);
  const onChangeMakeShared = (e) => setMakeShared(!isCorporate);

  const onSave = () => {
    saveThirdParty(
      urlValue,
      loginValue,
      passwordValue,
      auth_key,
      isCorporate,
      customerTitle,
      provider_key,
      provider_id
    );
  };

  const onReconnect = () => {
    openConnectWindow(title);
  };

  const isAccount = !!link;
  const showUrlField = title === "WebDav" || title === "SharePoint";

  return (
    <StyledConnectedDialog>
      <ModalDialog visible={visible} zIndex={310} onClose={onClose}>
        <ModalDialog.Header>{t("ConnectingAccount")}</ModalDialog.Header>
        <ModalDialog.Body>
          {isAccount ? (
            <div className="dialog-form-container">
              <Text className="dialog-form-text">{t("Account")}</Text>
              <Button
                label={t("Reconnect")}
                size="medium"
                onClick={onReconnect}
                scale
              />
            </div>
          ) : (
            <>
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
                <PasswordInput
                  className="dialog-form-input"
                  simpleView
                  passwordSettings={{ minLength: 0 }}
                  value={passwordValue}
                  onChange={onChangePassword}
                />
              </div>
            </>
          )}

          <div className="dialog-form-container">
            <Text className="dialog-form-text">{t("ConnectFolderTitle")}</Text>
            <TextInput
              scale
              value={`${customerTitle} ${t("Directory")}`}
              onChange={onChangeFolderName}
            />
          </div>
          <div className="dialog-form-container">
            <Text className="dialog-form-text" />
            <Checkbox
              label={t("ConnectMakeShared")}
              isChecked={isCorporate}
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
