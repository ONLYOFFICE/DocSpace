import React, { useState, useEffect, useCallback } from "react";
import toastr from "studio/toastr";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Checkbox from "@appserver/components/checkbox";
import TextInput from "@appserver/components/text-input";
import PasswordInput from "@appserver/components/password-input";
import FieldContainer from "@appserver/components/field-container";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { runInAction } from "mobx";

const PureConnectDialogContainer = (props) => {
  const {
    visible,
    t,
    tReady,
    item,
    treeFolders,
    fetchThirdPartyProviders,
    myFolderId,
    commonFolderId,
    providers,
    selectedFolderId,
    selectedFolderFolders,
    fetchFiles,
    getOAuthToken,
    saveThirdParty,
    openConnectWindow,
    setConnectDialogVisible,
    personal,
    getSubfolders,
  } = props;
  const {
    corporate,
    title,
    link,
    token,
    provider_id,
    provider_key,
    key,
  } = item;

  const provider = providers.find((el) => el.provider_id === item.provider_id);
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

  const onClose = useCallback(() => {
    !isLoading && setConnectDialogVisible(false);
  }, [isLoading, setConnectDialogVisible]);

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
      urlValue,
      loginValue,
      passwordValue,
      oAuthToken,
      isCorporate,
      customerTitle,
      provider_key || key,
      provider_id
    )
      .then(async () => {
        const folderId = isCorporate ? commonFolderId : myFolderId;
        const subfolders = await getSubfolders(folderId);
        const node = treeFolders.find((x) => x.id === folderId);

        runInAction(() => (node.folders = subfolders));

        await fetchThirdPartyProviders();
      })
      .catch((err) => {
        onClose();
        toastr.error(err);
        setIsLoading(false);
      })
      .finally(() => {
        onClose();
        setIsLoading(false);
      });
  }, [
    commonFolderId,
    customerTitle,
    fetchFiles,
    fetchThirdPartyProviders,
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
    showUrlField,
    treeFolders,
    urlValue,
    saveThirdParty,
  ]);

  const onReconnect = () => {
    let authModal = window.open("", "Authorization", "height=600, width=1020");
    openConnectWindow(title, authModal).then((modal) =>
      getOAuthToken(modal).then((token) => {
        authModal.close();
        setToken(token);
      })
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

  useEffect(() => {
    return setToken(token);
  }, [setToken, token]);

  return (
    <ModalDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={310}
      onClose={onClose}
    >
      <ModalDialog.Header>
        {t("Translations:ConnectingAccount")}
      </ModalDialog.Header>
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
                errorMessage={t("Common:RequiredField")}
              >
                <TextInput
                  isAutoFocussed={true}
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
              errorMessage={t("Common:RequiredField")}
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
              labelText={t("Common:Password")}
              isRequired
              isVertical
              hasError={!isPasswordValid}
              errorMessage={t("Common:RequiredField")}
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
          errorMessage={t("Common:RequiredField")}
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
        {!personal && (
          <Checkbox
            label={t("ConnectMakeShared")}
            isChecked={isCorporate}
            onChange={onChangeMakeShared}
            isDisabled={isLoading}
          />
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          tabIndex={5}
          label={t("Common:SaveButton")}
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

const ConnectDialog = withTranslation([
  "ConnectDialog",
  "Common",
  "Translations",
])(PureConnectDialogContainer);

export default inject(
  ({
    auth,
    filesStore,
    settingsStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
  }) => {
    const {
      providers,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
    } = settingsStore.thirdPartyStore;
    const { fetchFiles } = filesStore;
    const { getOAuthToken, personal } = auth.settingsStore;

    const {
      treeFolders,
      myFolderId,
      commonFolderId,
      getSubfolders,
    } = treeFoldersStore;
    const { id, folders } = selectedFolderStore;
    const {
      connectDialogVisible: visible,
      setConnectDialogVisible,
      connectItem: item,
    } = dialogsStore;

    return {
      selectedFolderId: id,
      selectedFolderFolders: folders,
      treeFolders,
      myFolderId,
      commonFolderId,
      providers,
      visible,
      item,

      fetchFiles,
      getOAuthToken,
      getSubfolders,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
      setConnectDialogVisible,

      personal,
    };
  }
)(observer(ConnectDialog));
