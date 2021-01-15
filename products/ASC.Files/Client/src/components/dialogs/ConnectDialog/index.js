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
import { utils as commonUtils, toastr } from "asc-web-common";
import {
  fetchFiles,
  fetchThirdPartyProviders,
  fetchTreeFolders,
  getOAuthToken,
  openConnectWindow,
  saveThirdParty,
  setIsLoading,
  setSelectedNode,
  setTreeFolders,
  setUpdateTree,
} from "../../../store/files/actions";
import {
  getTreeFolders,
  loopTreeFolders,
  getMyFolderId,
  getCommonFolderId,
  getThirdPartyProviders,
} from "../../../store/files/selectors";
import { withTranslation, I18nextProvider } from "react-i18next";
import { connect } from "react-redux";
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
  const {
    onClose,
    visible,
    t,
    item,
    fetchFiles,
    treeFolders,
    setUpdateTree,
    setTreeFolders,
    fetchThirdPartyProviders,
    fetchTreeFolders,
    myFolderId,
    commonFolderId,
    setIsLoading,
    setSelectedNode,
    providers,
  } = props;
  const { corporate, title, link, provider_id, provider_key } = item;

  const provider = providers.find(
    (el) => el.provider_key === item.provider_key
  );
  const folderTitle = provider ? provider.customer_title : title;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [customerTitle, setCustomerTitleValue] = useState(folderTitle);
  const [isCorporate, setMakeShared] = useState(!!corporate);

  const onChangeUrl = (e) => setUrlValue(e.target.value);
  const onChangeLogin = (e) => setLoginValue(e.target.value);
  const onChangePassword = (e) => setPasswordValue(e.target.value);
  const onChangeFolderName = (e) => setCustomerTitleValue(e.target.value);
  const onChangeMakeShared = (e) => setMakeShared(!isCorporate);

  const isEmptyField = [
    urlValue,
    loginValue,
    passwordValue,
    customerTitle,
  ].some((el) => el.trim().length === 0);

  const onSave = () => {
    if (isEmptyField) return toastr.error(t("EmptyField"));

    onClose();
    setIsLoading(true);
    saveThirdParty(
      null,
      null,
      null,
      null,
      isCorporate,
      customerTitle,
      provider_key,
      provider_id
    )
      .then((folderData) => {
        //const folderId = isCorporate ? commonFolderId : myFolderId;

        fetchTreeFolders().then((data) => {
          const commonFolder = data.treeFolders.find(
            (x) => x.id === commonFolderId
          );
          const myFolder = data.treeFolders.find((x) => x.id === myFolderId);

          const newTreeFolders = treeFolders;

          loopTreeFolders(
            myFolder.pathParts,
            newTreeFolders,
            myFolder.folders,
            myFolder.foldersCount,
            !isCorporate ? folderData : null
          );

          loopTreeFolders(
            commonFolder.pathParts,
            newTreeFolders,
            commonFolder.folders,
            commonFolder.foldersCount,
            isCorporate ? folderData : null
          );
          setTreeFolders(newTreeFolders);
          setUpdateTree(true);
          fetchThirdPartyProviders();
          setSelectedNode([`${folderData.id}`]);
          fetchFiles(folderData.id);
        });
      })
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  const onReconnect = () => {
    let authModal = window.open("", "Authorization", "height=600, width=1020");
    openConnectWindow(title, authModal).then(
      (modal) => getOAuthToken(modal) //.then((token) => setToken(token))
    );
  };

  const onKeyUpHandler = (e) => {
    if (e.keyCode === 13) onSave();
  };

  const isAccount = !!link;
  const showUrlField = title === "WebDav" || title === "SharePoint";

  return (
    <StyledConnectedDialog onKeyUp={onKeyUpHandler} autoFocus>
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
                  <TextInput
                    tabIndex={1}
                    scale
                    value={urlValue}
                    onChange={onChangeUrl}
                  />
                </div>
              )}

              <div className="dialog-form-container">
                <Text className="dialog-form-text">{t("Login")}</Text>
                <TextInput
                  tabIndex={2}
                  scale
                  value={loginValue}
                  onChange={onChangeLogin}
                />
              </div>
              <div className="dialog-form-container">
                <Text className="dialog-form-text">{t("Password")}</Text>
                <PasswordInput
                  className="dialog-form-input"
                  tabIndex={3}
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
              tabIndex={4}
              scale
              value={`${customerTitle}`}
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
          <Button
            tabIndex={5}
            label={t("SaveButton")}
            size="big"
            primary
            onClick={onSave}
          />
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

const mapStateToProps = (state) => {
  return {
    treeFolders: getTreeFolders(state),
    myFolderId: getMyFolderId(state),
    commonFolderId: getCommonFolderId(state),
    providers: getThirdPartyProviders(state),
  };
};

export default connect(mapStateToProps, {
  fetchFiles,
  setUpdateTree,
  setTreeFolders,
  fetchThirdPartyProviders,
  fetchTreeFolders,
  setIsLoading,
  setSelectedNode,
})(ConnectDialog);
