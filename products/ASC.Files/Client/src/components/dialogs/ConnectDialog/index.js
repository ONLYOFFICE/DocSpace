import React, { useState, useEffect } from "react";
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
import { utils as commonUtils } from "asc-web-common";
import {
  saveThirdParty,
  openConnectWindow,
} from "../../../store/files/actions";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "ConnectDialog",
  localesPath: "dialogs/ConnectDialog",
});

const { changeLanguage } = commonUtils;
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

const PureConnectDialogContainer = (props) => {
  const { onClose, visible, t, item } = props;
  const { corporate, title, link, token, provider_id, provider_key } = item;

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
      token,
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

const ConnectDialogContainer = withTranslation()(PureConnectDialogContainer);

const ConnectDialog = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <ConnectDialogContainer {...props} />
    </I18nextProvider>
  );
};

export default ConnectDialog;
