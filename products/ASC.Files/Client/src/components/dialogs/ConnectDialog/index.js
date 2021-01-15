import React, { useState, useEffect } from "react";
import styled from "styled-components";
import {
  ModalDialog,
  TextInput,
  PasswordInput,
  Button,
  Checkbox,
  Text,
  FieldContainer,
  utils,
} from "asc-web-components";
import { utils as commonUtils } from "asc-web-common";
import {
  fetchThirdPartyProviders,
  fetchTreeFolders,
  getOAuthToken,
  openConnectWindow,
  saveThirdParty,
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
  .field-label {
    color: #333;
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
    treeFolders,
    setUpdateTree,
    setTreeFolders,
    fetchThirdPartyProviders,
    fetchTreeFolders,
    myFolderId,
    commonFolderId,
    providers,
  } = props;
  const { corporate, title, link, token, provider_id, provider_key } = item;

  const provider = providers.find(
    (el) => el.provider_key === item.provider_key
  );
  const folderTitle = provider ? provider.customer_title : title;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [customerTitle, setCustomerTitleValue] = useState(folderTitle);
  const [isCorporate, setMakeShared] = useState(!!corporate);
  const [oAuthToken, setToken] = useState(token);
  const [errorText, setErrorText] = useState("");

  const [isTitleValid, setIsTitleValid] = useState(true);
  const [isUrlValid, setIsUrlValid] = useState(true);
  const [isLoginValid, setIsLoginValid] = useState(true);
  const [isPasswordValid, setIsPasswordValid] = useState(true);

  const [isLoading, setIsLoading] = useState(false);

  const isAccount = !!link;
  const showUrlField = title === "WebDav" || title === "SharePoint";

  const onChangeUrl = (e) => {
    setIsUrlValid(true);
    setUrlValue(e.target.value);
  };
  const onChangeLogin = (e) => {
    setIsLoginValid(true);
    setLoginValue(e.target.value);
  };
  const onChangePassword = (e) => {
    setIsPasswordValid(true);
    setPasswordValue(e.target.value);
  };
  const onChangeFolderName = (e) => {
    setIsTitleValid(true);
    setCustomerTitleValue(e.target.value);
  };
  const onChangeMakeShared = () => setMakeShared(!isCorporate);

  const onSave = () => {
    const isTitleValid = !!customerTitle.trim();
    const isUrlValid = !!urlValue.trim();
    const isLoginValid = !!loginValue.trim();
    const isPasswordValid = !!passwordValue.trim();

    if (link) {
      if (!isTitleValid) {
        setIsTitleValid(!!customerTitle.trim());
        return;
      }
    } else {
      if (
        !isTitleValid ||
        !isLoginValid ||
        !isPasswordValid ||
        (showUrlField && !isUrlValid)
      ) {
        setIsTitleValid(isTitleValid);
        showUrlField && setIsUrlValid(isUrlValid);
        setIsLoginValid(isLoginValid);
        setIsPasswordValid(isPasswordValid);
        return;
      }
    }

    setIsLoading(true);
    saveThirdParty(
      null,
      null,
      null,
      oAuthToken,
      isCorporate,
      customerTitle,
      provider_key,
      provider_id
    )
      .then((folderData) => {
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
          onClose();
        });
      })
      .catch((err) => setErrorText(err))
      .finally(() => setIsLoading(false));
  };

  const onReconnect = () => {
    let authModal = window.open("", "Authorization", "height=600, width=1020");
    openConnectWindow(title, authModal).then((modal) =>
      getOAuthToken(modal).then((token) => setToken(token))
    );
  };

  const onKeyUpHandler = (e) => {
    if (e.keyCode === 13) onSave();
  };

  console.log("errorText", errorText);

  return (
    <StyledConnectedDialog onKeyUp={onKeyUpHandler} autoFocus>
      <ModalDialog visible={visible} zIndex={310} onClose={onClose}>
        <ModalDialog.Header>{t("ConnectingAccount")}</ModalDialog.Header>
        <ModalDialog.Body>
          {isAccount ? (
            <FieldContainer labelVisible labelText={t("Account")} isVertical>
              <Button
                label={t("Reconnect")}
                size="medium"
                onClick={onReconnect}
                scale
                isDisabled={isLoading}
              />
            </FieldContainer>
          ) : (
            <>
              {showUrlField && (
                <FieldContainer
                  labelVisible
                  labelText={t("ConnectionUrl")}
                  isVertical
                  hasError={!isUrlValid}
                  errorMessage={errorText ? "" : t("RequiredFieldMessage")}
                >
                  <TextInput
                    isDisabled={isLoading}
                    tabIndex={1}
                    scale
                    value={urlValue}
                    onChange={onChangeUrl}
                  />
                </FieldContainer>
              )}

              <FieldContainer
                labelText={t("Login")}
                isVertical
                hasError={!isLoginValid}
                errorMessage={errorText ? "" : t("RequiredFieldMessage")}
              >
                <TextInput
                  isDisabled={isLoading}
                  tabIndex={2}
                  scale
                  value={loginValue}
                  onChange={onChangeLogin}
                />
              </FieldContainer>
              <FieldContainer
                labelText={t("Password")}
                isVertical
                hasError={!isPasswordValid}
                errorMessage={errorText ? "" : t("RequiredFieldMessage")}
              >
                <PasswordInput
                  isDisabled={isLoading}
                  className="dialog-form-input"
                  tabIndex={3}
                  simpleView
                  passwordSettings={{ minLength: 0 }}
                  value={passwordValue}
                  onChange={onChangePassword}
                />
              </FieldContainer>
            </>
          )}

          <FieldContainer
            labelText={t("ConnectFolderTitle")}
            isVertical
            hasError={!isTitleValid || !!errorText}
            errorMessage={errorText ? errorText : t("RequiredFieldMessage")}
          >
            <TextInput
              tabIndex={4}
              scale
              value={`${customerTitle}`}
              onChange={onChangeFolderName}
              isDisabled={isLoading}
            />
          </FieldContainer>
          <Text className="dialog-form-text" />
          <Checkbox
            label={t("ConnectMakeShared")}
            isChecked={isCorporate}
            onChange={onChangeMakeShared}
            isDisabled={isLoading}
          />
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            tabIndex={5}
            label={t("SaveButton")}
            size="big"
            primary
            onClick={onSave}
            isDisabled={isLoading}
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
  setUpdateTree,
  setTreeFolders,
  fetchThirdPartyProviders,
  fetchTreeFolders,
})(ConnectDialog);
