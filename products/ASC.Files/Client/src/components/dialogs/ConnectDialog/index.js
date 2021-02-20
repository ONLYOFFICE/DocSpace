import React, { useState, useEffect, useCallback } from "react";
import {
  ModalDialog,
  TextInput,
  PasswordInput,
  Button,
  Checkbox,
  FieldContainer,
  toastr,
} from "asc-web-components";
import { loopTreeFolders } from "../../../helpers/files-helpers";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const PureConnectDialogContainer = (props) => {
  const {
    visible,
    t,
    item,
    treeFolders,
    setTreeFolders,
    fetchThirdPartyProviders,
    fetchTreeFolders,
    myFolderId,
    commonFolderId,
    providers,
    selectedFolderId,
    selectedFolderFolders,
    fetchFiles,
    getOAuthToken,
    saveThirdParty,
    openConnectWindow,
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
  const onClose = useCallback(() => !isLoading && props.onClose(), [
    isLoading,
    props,
  ]);

  const onSave = useCallback(() => {
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
          fetchThirdPartyProviders();

          const newFolder =
            selectedFolderFolders &&
            selectedFolderFolders.find((x) => x.id === folderData.id);
          if (newFolder)
            fetchFiles(selectedFolderId).then(() => {
              onClose();
              setIsLoading(false);
            });
          else {
            onClose();
            setIsLoading(false);
          }
        });
      })
      .catch((err) => {
        toastr.error(err);
        setIsLoading(false);
      });
  }, [
    commonFolderId,
    customerTitle,
    fetchFiles,
    fetchThirdPartyProviders,
    fetchTreeFolders,
    isCorporate,
    link,
    loginValue,
    myFolderId,
    oAuthToken,
    onClose,
    passwordValue,
    provider_id,
    provider_key,
    selectedFolderFolders,
    selectedFolderId,
    setTreeFolders,
    showUrlField,
    treeFolders,
    urlValue,
    saveThirdParty,
  ]);

  const onReconnect = () => {
    let authModal = window.open("", "Authorization", "height=600, width=1020");
    openConnectWindow(title, authModal).then((modal) =>
      getOAuthToken(modal).then((token) => setToken(token))
    );
  };

  const onKeyUpHandler = useCallback(
    (e) => {
      if (e.keyCode === 13) onSave();
    },
    [onSave]
  );

  useEffect(() => {
    window.addEventListener("keyup", onKeyUpHandler);
    return () => window.removeEventListener("keyup", onKeyUpHandler);
  }, [onKeyUpHandler]);

  return (
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
                isRequired
                labelText={t("ConnectionUrl")}
                isVertical
                hasError={!isUrlValid}
                errorMessage={t("RequiredFieldMessage")}
              >
                <TextInput
                  hasError={!isUrlValid}
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
              isRequired
              isVertical
              hasError={!isLoginValid}
              errorMessage={t("RequiredFieldMessage")}
            >
              <TextInput
                hasError={!isLoginValid}
                isDisabled={isLoading}
                tabIndex={2}
                scale
                value={loginValue}
                onChange={onChangeLogin}
              />
            </FieldContainer>
            <FieldContainer
              labelText={t("Password")}
              isRequired
              isVertical
              hasError={!isPasswordValid}
              errorMessage={t("RequiredFieldMessage")}
            >
              <PasswordInput
                hasError={!isPasswordValid}
                isDisabled={isLoading}
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
          isRequired
          isVertical
          hasError={!isTitleValid}
          errorMessage={t("RequiredFieldMessage")}
        >
          <TextInput
            hasError={!isTitleValid}
            isDisabled={isLoading}
            tabIndex={4}
            scale
            value={`${customerTitle}`}
            onChange={onChangeFolderName}
          />
        </FieldContainer>
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
          isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

const ConnectDialog = withTranslation("ConnectDialog")(
  PureConnectDialogContainer
);

export default inject(
  ({ filesStore, settingsStore, treeFoldersStore, selectedFolderStore }) => {
    const {
      providers,
      getOAuthToken,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
    } = settingsStore.thirdPartyStore;
    const { fetchFiles } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      myFolderId,
      commonFolderId,
      fetchTreeFolders,
    } = treeFoldersStore;
    const { id, folders } = selectedFolderStore;

    return {
      selectedFolderId: id,
      selectedFolderFolders: folders,
      treeFolders,
      myFolderId,
      commonFolderId,
      providers,

      fetchFiles,
      setTreeFolders,
      getOAuthToken,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
      fetchTreeFolders,
    };
  }
)(observer(ConnectDialog));
