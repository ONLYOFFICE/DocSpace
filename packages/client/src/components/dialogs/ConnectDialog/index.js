import React, { useState, useEffect, useCallback } from "react";
import toastr from "@docspace/components/toast/toastr";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Checkbox from "@docspace/components/checkbox";
import TextInput from "@docspace/components/text-input";
import PasswordInput from "@docspace/components/password-input";
import FieldContainer from "@docspace/components/field-container";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { getOAuthToken } from "@docspace/common/utils";
import { saveSettingsThirdParty } from "@docspace/common/api/files";

const PureConnectDialogContainer = (props) => {
  const {
    visible,
    t,
    tReady,
    item,
    fetchThirdPartyProviders,
    providers,
    selectedFolderId,
    selectedFolderFolders,
    saveThirdParty,
    openConnectWindow,
    setConnectDialogVisible,
    personal,
    folderFormValidation,
    isConnectionViaBackupModule,
    roomCreation,
    setSaveThirdpartyResponse,
    isConnectDialogReconnect,
    setIsConnectDialogReconnect,
    saveAfterReconnectOAuth,
    setSaveAfterReconnectOAuth,
    setSelectedThirdPartyAccount,
  } = props;
  const { title, link, token, provider_id, provider_key, key } = item;

  const provider = providers.find((el) => el.provider_id === item.provider_id);
  const folderTitle = provider ? provider.customer_title : title;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [customerTitle, setCustomerTitleValue] = useState(folderTitle);

  const [oAuthToken, setToken] = useState(token);

  const [isTitleValid, setIsTitleValid] = useState(true);
  const [isUrlValid, setIsUrlValid] = useState(true);
  const [isLoginValid, setIsLoginValid] = useState(true);
  const [isPasswordValid, setIsPasswordValid] = useState(true);

  const [isLoading, setIsLoading] = useState(false);

  const isAccount = !!link;
  const showUrlField =
    provider_key === "WebDav" ||
    provider_key === "SharePoint" ||
    key === "WebDav" ||
    key === "SharePoint";

  const header = isConnectDialogReconnect
    ? t("Common:ReconnectStorage")
    : t("Translations:ConnectingAccount");

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
    let title = e.target.value;
    //const chars = '*+:"<>?|/'; TODO: think how to solve problem with interpolation escape values in i18n translate

    if (title.match(folderFormValidation)) {
      toastr.warning(t("Files:ContainsSpecCharacter"));
    }
    title = title.replace(folderFormValidation, "_");

    setCustomerTitleValue(title);
  };

  const onClose = useCallback(() => {
    !isLoading && setConnectDialogVisible(false);

    if (isConnectDialogReconnect) {
      setIsConnectDialogReconnect(false);
    }
  }, [
    isLoading,
    setConnectDialogVisible,
    isConnectDialogReconnect,
    setIsConnectDialogReconnect,
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

    if (isConnectionViaBackupModule) {
      saveSettingsThirdParty(
        urlValue,
        loginValue,
        passwordValue,
        oAuthToken,
        false,
        customerTitle,
        provider_key,
        provider_id
      )
        .then(() => {
          onClose();
          setSelectedThirdPartyAccount(null);
        })
        .catch((err) => {
          toastr.error(err);
        })
        .finally(() => {
          setIsLoading(false);
          onClose();
        });

      return;
    }

    saveThirdParty(
      urlValue,
      loginValue,
      passwordValue,
      oAuthToken,
      false,
      customerTitle,
      provider_key || key,
      provider_id,
      roomCreation
    )
      .then(async (res) => {
        setSaveThirdpartyResponse(res);
        toastr.success(t("SuccessfulConnectionOfAThirdParty"));
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
        setSaveAfterReconnectOAuth(false);
      });
  }, [
    customerTitle,
    fetchThirdPartyProviders,
    link,
    loginValue,
    oAuthToken,
    onClose,
    passwordValue,
    provider_id,
    provider_key,
    selectedFolderFolders,
    selectedFolderId,
    urlValue,
    saveThirdParty,
  ]);

  const onReconnect = () => {
    let authModal = window.open(
      "",
      t("Common:Authorization"),
      "height=600, width=1020"
    );
    openConnectWindow(provider_key, authModal).then((modal) =>
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

  useEffect(() => {
    if (saveAfterReconnectOAuth) {
      onSave();
    }
  }, [saveAfterReconnectOAuth]);

  return (
    <ModalDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={310}
      isLarge={!isAccount}
      autoMaxHeight
      onClose={onClose}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        {isAccount ? (
          <FieldContainer
            style={roomCreation ? { margin: "0" } : {}}
            labelVisible
            labelText={t("Account")}
            isVertical
          >
            <Button
              label={t("Reconnect")}
              size="normal"
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
                  id="connection-url-input"
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
                id="login-input"
                isAutoFocussed={!showUrlField}
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
              style={roomCreation ? { margin: "0" } : {}}
            >
              <PasswordInput
                id="password-input"
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
        {!(isConnectionViaBackupModule || roomCreation) && (
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
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="save"
          tabIndex={5}
          label={t("Common:SaveButton")}
          size="normal"
          primary
          scale={isAccount}
          onClick={onSave}
          isDisabled={isLoading}
          isLoading={isLoading}
        />
        <Button
          id="cancel"
          tabIndex={5}
          label={t("Common:CancelButton")}
          size="normal"
          scale={isAccount}
          onClick={onClose}
          isDisabled={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

const ConnectDialog = withTranslation([
  "ConnectDialog",
  "Common",
  "Translations",
  "Files",
])(PureConnectDialogContainer);

export default inject(
  ({ auth, settingsStore, selectedFolderStore, dialogsStore, backup }) => {
    const {
      providers,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
    } = settingsStore.thirdPartyStore;
    const { personal, folderFormValidation } = auth.settingsStore;

    const { id, folders } = selectedFolderStore;
    const {
      selectedThirdPartyAccount: backupConnectionItem,
      setSelectedThirdPartyAccount,
    } = backup;
    const {
      connectDialogVisible: visible,
      setConnectDialogVisible,
      connectItem,
      roomCreation,
      setSaveThirdpartyResponse,
      isConnectDialogReconnect,
      setIsConnectDialogReconnect,
      saveAfterReconnectOAuth,
      setSaveAfterReconnectOAuth,
    } = dialogsStore;

    const item = backupConnectionItem ?? connectItem;
    const isConnectionViaBackupModule = backupConnectionItem ? true : false;

    return {
      selectedFolderId: id,
      selectedFolderFolders: folders,
      providers,
      visible,
      item,
      roomCreation,
      setSaveThirdpartyResponse,
      folderFormValidation,
      isConnectionViaBackupModule,
      saveThirdParty,
      openConnectWindow,
      fetchThirdPartyProviders,
      setConnectDialogVisible,
      setSelectedThirdPartyAccount,
      personal,
      isConnectDialogReconnect,
      saveAfterReconnectOAuth,
      setSaveAfterReconnectOAuth,
      setIsConnectDialogReconnect,
    };
  }
)(observer(ConnectDialog));
