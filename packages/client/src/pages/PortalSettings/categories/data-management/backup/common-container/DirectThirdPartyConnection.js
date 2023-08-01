import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";
import AccessNoneReactSvgUrl from "PUBLIC_DIR/images/access.none.react.svg?url";
import React, { useEffect, useState, useReducer } from "react";
import Button from "@docspace/components/button";
import SelectFolderInput from "client/SelectFolderInput";
import SelectFileInput from "client/SelectFileInput";
import {
  getSettingsThirdParty,
  getThirdPartyCapabilities,
  saveSettingsThirdParty,
} from "@docspace/common/api/files";
import { StyledBackup } from "../StyledBackup";
import ComboBox from "@docspace/components/combobox";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import { ContextMenuButton } from "@docspace/components";
import DeleteThirdPartyDialog from "../../../../../../components/dialogs/DeleteThirdPartyDialog";
import { withTranslation } from "react-i18next";
import { getOAuthToken } from "@docspace/common/utils";

let accounts = [],
  capabilities;
const DirectThirdPartyConnection = (props) => {
  const {
    openConnectWindow,
    t,
    onSelectFolder,
    onClose,
    onClickInput,
    onSetLoadingData,
    isDisabled,
    isPanelVisible,
    isError,
    id,
    withoutBasicSelection,
    onSelectFile,
    isFileSelection = false,
    connectDialogVisible,
    setConnectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
    tReady,
    clearLocalStorage,
    setSelectedThirdPartyAccount,
    connectedThirdPartyAccount,
    selectedThirdPartyAccount,
    setConnectedThirdPartyAccount,
    buttonSize,
    isTheSameThirdPartyAccount,
  } = props;

  useEffect(() => {
    return () => {
      setSelectedThirdPartyAccount(null);
    };
  }, []);

  useEffect(() => {
    tReady && onSetInitialInfo();
  }, [tReady]);

  const onSetInitialInfo = async () => {
    try {
      const capabilities = await getThirdPartyCapabilities();
      onSetThirdPartySettings(connectedThirdPartyAccount, capabilities);
    } catch (e) {
      onSetThirdPartySettings();
      if (!e) return;
      toastr.error(e);
    }
  };

  useEffect(() => {
    selectedThirdPartyAccount === null &&
      !isInitialLoading &&
      updateAccountsInfo();
  }, [selectedThirdPartyAccount === null]);

  const initialState = {
    folderList: [],
    isLoading: false,
    isInitialLoading: true,
    isUpdatingInfo: false,
  };

  const [state, setState] = useReducer(
    (state, newState) => ({ ...state, ...newState }),
    initialState
  );

  const isDirectConnection = () => {
    return state.isUpdatingInfo;
  };
  const updateAccountsInfo = async () => {
    try {
      if (!isDirectConnection()) setState({ isUpdatingInfo: true });

      let account;
      [account, capabilities] = await Promise.all([
        getSettingsThirdParty(),
        getThirdPartyCapabilities(),
      ]);
      setConnectedThirdPartyAccount(account);
      onSetThirdPartySettings(account, capabilities);
    } catch (e) {
      onSetThirdPartySettings();

      if (!e) return;
      toastr.error(e);
    }
  };

  const onSetThirdPartySettings = async (connectedAccount, capabilities) => {
    try {
      accounts = [];

      let index = 0,
        selectedAccount = {};

      const setAccount = (providerKey, serviceTitle) => {
        const accountIndex =
          capabilities && capabilities.findIndex((x) => x[0] === providerKey);

        if (accountIndex === -1) return;

        const isConnected =
          connectedAccount?.providerKey === "WebDav"
            ? serviceTitle === connectedAccount?.title
            : capabilities[accountIndex][0] === connectedAccount?.providerKey;

        accounts.push({
          key: index.toString(),
          label: serviceTitle,
          title: serviceTitle,
          provider_key: capabilities[accountIndex][0],
          ...(capabilities[accountIndex][1] && {
            provider_link: capabilities[accountIndex][1],
          }),
          connected: isConnected,
          ...(isConnected && {
            provider_id: connectedAccount?.providerId,
          }),
        });

        if (isConnected) {
          selectedAccount = { ...accounts[index] };
        }

        index++;
      };

      setAccount("GoogleDrive", t("Translations:TypeTitleGoogle"));
      setAccount("Box", t("Translations:TypeTitleBoxNet"));
      setAccount("DropboxV2", t("Translations:TypeTitleDropBox"));
      setAccount("SharePoint", t("Translations:TypeTitleSharePoint"));
      setAccount("OneDrive", t("Translations:TypeTitleSkyDrive"));
      setAccount("WebDav", "Nextcloud");
      setAccount("WebDav", "ownCloud");
      setAccount("kDrive", t("Translations:TypeTitlekDrive"));
      setAccount("Yandex", t("Translations:TypeTitleYandex"));
      setAccount("WebDav", t("Translations:TypeTitleWebDav"));

      setSelectedThirdPartyAccount(
        Object.keys(selectedAccount).length !== 0
          ? selectedAccount
          : { ...accounts[0] }
      );

      setState({
        isLoading: false,
        isUpdatingInfo: false,
        isInitialLoading: false,
        folderList: connectedAccount ?? [],
      });
    } catch (e) {
      setState({
        isLoading: false,
        isInitialLoading: false,
        isUpdatingInfo: false,
      });
      if (!e) return;
      toastr.error(e);
    }
  };

  const onConnect = () => {
    clearLocalStorage();

    const {
      provider_key,
      provider_link: directConnection,
    } = selectedThirdPartyAccount;

    if (directConnection) {
      let authModal = window.open(
        "",
        t("Common:Authorization"),
        "height=600, width=1020"
      );

      openConnectWindow(provider_key, authModal)
        .then((modal) => getOAuthToken(modal))
        .then((token) => {
          authModal.close();
          saveSettings(token);
        })
        .catch((e) => {
          if (!e) return;
          toastr.error(e);
          console.error(e);
        });
    } else {
      setConnectDialogVisible(true);
    }
  };

  const saveSettings = async (
    token = "",
    urlValue = "",
    loginValue = "",
    passwordValue = ""
  ) => {
    const { label, provider_key, provider_id } = selectedThirdPartyAccount;
    setState({ isLoading: true, isUpdatingInfo: true });
    connectDialogVisible && setConnectDialogVisible(false);
    onSelectFolder && onSelectFolder("");

    try {
      await saveSettingsThirdParty(
        urlValue,
        loginValue,
        passwordValue,
        token,
        false,
        label,
        provider_key,
        provider_id
      );

      setSelectedThirdPartyAccount(null);
    } catch (e) {
      setState({ isLoading: false, isUpdatingInfo: false });
      toastr.error(e);
    }
  };

  const onSelectAccount = (options) => {
    const key = options.key;
    setSelectedThirdPartyAccount({ ...accounts[+key] });
  };

  const onDisconnect = () => {
    clearLocalStorage();
    setDeleteThirdPartyDialogVisible(true);
  };
  const getContextOptions = () => {
    return [
      {
        key: "connection-settings",
        label: t("Reconnect"),
        onClick: onConnect,
        disabled: false,
        icon: RefreshReactSvgUrl,
      },
      {
        key: "Disconnect-settings",
        label: t("Common:Disconnect"),
        onClick: onDisconnect,
        disabled: selectedThirdPartyAccount?.connected ? false : true,
        icon: AccessNoneReactSvgUrl,
      },
    ];
  };

  const { isLoading, folderList, isInitialLoading, isUpdatingInfo } = state;

  const fileSelection = isFileSelection ? (
    <SelectFileInput
      passedFoldersTree={[folderList]}
      onClose={onClose}
      onSelectFile={onSelectFile}
      onClickInput={onClickInput}
      isPanelVisible={isPanelVisible}
      searchParam=".gz"
      filesListTitle={t("Settings:SelectFileInGZFormat")}
      withoutResetFolderTree
      isArchiveOnly
      isDisabled={
        isLoading ||
        accounts.length === 0 ||
        folderList.length === 0 ||
        isDisabled
      }
      isError={isError}
    />
  ) : (
    <SelectFolderInput
      id={id}
      onSelectFolder={onSelectFolder}
      name={"thirdParty"}
      onClose={onClose}
      onClickInput={onClickInput}
      onSetLoadingData={onSetLoadingData}
      isDisabled={
        isLoading ||
        accounts.length === 0 ||
        folderList.length === 0 ||
        isDisabled
      }
      isPanelVisible={isPanelVisible}
      isError={isError}
      passedFoldersTree={[folderList]}
      withoutBasicSelection={withoutBasicSelection}
      isWaitingUpdate={isInitialLoading || isUpdatingInfo ? true : false}
    />
  );

  return (
    <StyledBackup
      isConnectedAccount={
        connectedThirdPartyAccount && isTheSameThirdPartyAccount
      }
    >
      <div className="backup_connection">
        <ComboBox
          options={accounts}
          selectedOption={{
            key: 0,
            label: selectedThirdPartyAccount?.label,
          }}
          onSelect={onSelectAccount}
          noBorder={false}
          scaledOptions
          dropDownMaxHeight={300}
          tabIndex={1}
          showDisabledItems
          isDisabled={
            !tReady ||
            isDisabled ||
            isInitialLoading ||
            isLoading ||
            accounts.length === 0
          }
        />

        {connectedThirdPartyAccount?.id && isTheSameThirdPartyAccount && (
          <ContextMenuButton
            zIndex={402}
            className="backup_third-party-context"
            iconName={VerticalDotsReactSvgUrl}
            size={15}
            getData={getContextOptions}
            isDisabled={
              isDisabled ||
              isInitialLoading ||
              isLoading ||
              accounts.length === 0
            }
            displayIconBorder
          />
        )}
      </div>

      {!connectedThirdPartyAccount?.id || !isTheSameThirdPartyAccount ? (
        <Button
          primary
          label={t("Common:Connect")}
          onClick={onConnect}
          size={buttonSize}
        />
      ) : (
        fileSelection
      )}
      {deleteThirdPartyDialogVisible && (
        <DeleteThirdPartyDialog
          updateInfo={updateAccountsInfo}
          key="thirdparty-delete-dialog"
          isConnectionViaBackupModule
        />
      )}
    </StyledBackup>
  );
};

export default inject(({ backup, dialogsStore, settingsStore }) => {
  const {
    clearLocalStorage,
    setSelectedThirdPartyAccount,
    selectedThirdPartyAccount,
    connectedThirdPartyAccount,
    setConnectedThirdPartyAccount,
    isTheSameThirdPartyAccount,
  } = backup;
  const { openConnectWindow } = settingsStore.thirdPartyStore;

  const {
    connectDialogVisible,
    setConnectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
  } = dialogsStore;

  return {
    isTheSameThirdPartyAccount,
    clearLocalStorage,
    openConnectWindow,
    setConnectDialogVisible,
    connectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
    setSelectedThirdPartyAccount,
    selectedThirdPartyAccount,
    connectedThirdPartyAccount,
    setConnectedThirdPartyAccount,
  };
})(
  withTranslation(["ConnectDialog", "Settings", "Common", "Translations"])(
    observer(DirectThirdPartyConnection)
  )
);
