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
import Text from "@docspace/components/text";
import ComboBox from "@docspace/components/combobox";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import ConnectDialog from "../../../../../../components/dialogs/ConnectDialog";
import { ContextMenuButton } from "@docspace/components";
import DeleteThirdPartyDialog from "../../../../../../components/dialogs/DeleteThirdPartyDialog";
import { withTranslation } from "react-i18next";

let accounts = [],
  connectedAccount,
  capabilities;
const DirectThirdPartyConnection = (props) => {
  const {
    openConnectWindow,
    getOAuthToken,
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
  } = props;

  useEffect(() => {
    tReady && updateAccountsInfo(true);
  }, [tReady]);

  const initialState = {
    selectedAccount: {},
    folderList: [],
    isLoading: false,
    isInitialLoading: true,
    isUpdatingInfo: false,
  };

  const [state, setState] = useReducer(
    (state, newState) => ({ ...state, ...newState }),
    initialState
  );

  const updateAccountsInfo = async (isMount = false) => {
    if (!isMount && !state.isLoading && !state.isUpdatingInfo) {
      onSelectFolder && onSelectFolder("");
      setState({ isLoading: true, isUpdatingInfo: true });
    }

    try {
      [connectedAccount, capabilities] = await Promise.all([
        getConnectedAccounts(),
        getThirdPartyCapabilities(),
      ]);

      onSetThirdPartySettings(
        connectedAccount?.providerKey,
        connectedAccount?.providerId,
        connectedAccount,
        capabilities
      );
    } catch (e) {
      onSetThirdPartySettings();

      if (!e) return;
      toastr.error(e);
    }
  };

  const onSetThirdPartySettings = async (
    connectedProviderKey,
    connectedProviderId,
    account,
    capabilities
  ) => {
    try {
      !state.isLoading &&
        !state.isInitialLoading &&
        setState({ isLoading: true });

      accounts = [];

      let index = 0,
        selectedAccount = {};

      const getCapability = (providerId) => {
        return (
          capabilities && capabilities.findIndex((x) => x[0] === providerId)
        );
      };
      const setAccount = (providerKey, serviceTitle) => {
        const accountIndex = getCapability(providerKey);
        if (accountIndex === -1) return;

        const isConnected =
          connectedProviderKey === "WebDav"
            ? serviceTitle === account.title
            : capabilities[accountIndex][0] === connectedProviderKey;

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
            provider_id: connectedProviderId,
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

      setState({
        isLoading: false,
        isUpdatingInfo: false,
        isInitialLoading: false,
        selectedAccount:
          Object.keys(selectedAccount).length !== 0
            ? selectedAccount
            : { ...accounts[0] },
        folderList: account ? account : [],
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
    const { provider_link, provider_key } = state.selectedAccount;

    const directConnection = provider_link;

    if (directConnection) {
      let authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );

      openConnectWindow(provider_key, authModal)
        .then(getOAuthToken)
        .then((token) => {
          saveSettings(token);
          authModal.close();
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

  const getConnectedAccounts = async () => {
    return await getSettingsThirdParty();
  };

  const saveSettings = async (
    token = "",
    urlValue = "",
    loginValue = "",
    passwordValue = ""
  ) => {
    const { label, provider_key, provider_id } = state.selectedAccount;
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

      updateAccountsInfo();
    } catch (e) {
      setState({ isLoading: false, isUpdatingInfo: false });
      toastr.error(e);
    }
  };

  const onSelectAccount = (options) => {
    const key = options.key;

    setState({ selectedAccount: { ...accounts[+key] } });
  };

  const onReconnect = () => {
    clearLocalStorage();

    const { provider_link } = state.selectedAccount;

    const directConnection = provider_link;

    if (directConnection) {
      let authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );
      openConnectWindow(selectedAccount.provider_key, authModal).then((modal) =>
        getOAuthToken(modal).then((token) => {
          authModal.close();
          saveSettings(token);
        })
      );
    } else {
      setConnectDialogVisible(true);
    }
  };

  const onDisconnect = () => {
    clearLocalStorage();
    setDeleteThirdPartyDialogVisible(true);
  };
  const getContextOptions = () => {
    return [
      {
        key: "connection-settings",
        label: selectedAccount.connected
          ? t("Common:Reconnect")
          : t("Common:Connect"),
        onClick: selectedAccount.connected ? onReconnect : onConnect,
        disabled: false,
        icon: "/static/images/refresh.react.svg",
      },
      {
        key: "Disconnect-settings",
        label: t("Common:Disconnect"),
        onClick: onDisconnect,
        disabled: selectedAccount.connected ? false : true,
        icon: "/static/images/access.none.react.svg",
      },
    ];
  };

  const {
    selectedAccount,
    isLoading,
    folderList,
    isInitialLoading,
    isUpdatingInfo,
  } = state;

  return (
    <StyledBackup>
      <div className="backup_connection">
        <ComboBox
          className="backup_third-party-combo"
          options={accounts}
          selectedOption={{
            key: 0,
            label: selectedAccount?.label,
          }}
          onSelect={onSelectAccount}
          noBorder={false}
          scaledOptions
          dropDownMaxHeight={300}
          tabIndex={1}
          isDisabled={
            !tReady ||
            isDisabled ||
            isInitialLoading ||
            isLoading ||
            accounts.length === 0
          }
        />

        <ContextMenuButton
          zIndex={402}
          className="backup_third-party-context"
          iconName="images/vertical-dots.react.svg"
          size={15}
          getData={getContextOptions}
          isDisabled={
            isDisabled || isInitialLoading || isLoading || accounts.length === 0
          }
          isNeedBorder
        />
      </div>

      {isFileSelection ? (
        <SelectFileInput
          foldersList={[folderList]}
          onClose={onClose}
          onSelectFile={onSelectFile}
          onClickInput={onClickInput}
          isPanelVisible={isPanelVisible}
          searchParam=".gz"
          filesListTitle={t("SelectFileInGZFormat")}
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
          foldersList={[folderList]}
          withoutBasicSelection={withoutBasicSelection}
          isWaitingUpdate={isInitialLoading || isUpdatingInfo ? true : false}
        />
      )}

      {connectDialogVisible && (
        <ConnectDialog
          passedItem={selectedAccount}
          updateInfo={updateAccountsInfo}
          isConnectionViaBackupModule
        />
      )}

      {deleteThirdPartyDialogVisible && (
        <DeleteThirdPartyDialog
          updateInfo={updateAccountsInfo}
          key="thirdparty-delete-dialog"
          item={selectedAccount}
          isConnectionViaBackupModule
        />
      )}
    </StyledBackup>
  );
};

export default inject(({ auth, backup, dialogsStore, settingsStore }) => {
  const { commonThirdPartyList, clearLocalStorage } = backup;
  const { openConnectWindow } = settingsStore.thirdPartyStore;
  const { getOAuthToken } = auth.settingsStore;
  const {
    connectDialogVisible,
    setConnectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
  } = dialogsStore;

  return {
    clearLocalStorage,
    commonThirdPartyList,
    openConnectWindow,
    getOAuthToken,
    setConnectDialogVisible,
    connectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
  };
})(
  withTranslation(["Settings", "Common", "Translations"])(
    observer(DirectThirdPartyConnection)
  )
);
