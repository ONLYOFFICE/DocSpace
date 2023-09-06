import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";
import AccessNoneReactSvgUrl from "PUBLIC_DIR/images/access.none.react.svg?url";
import React, { useEffect, useReducer } from "react";
import Button from "@docspace/components/button";
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
import { getOAuthToken } from "@docspace/common/utils";
import FilesSelectorInput from "SRC_DIR/components/FilesSelectorInput";
import { useTranslation } from "react-i18next";
let accounts = [],
  capabilities;

const initialState = {
  folderList: {},
  isLoading: false,
  isInitialLoading: true,
  isUpdatingInfo: false,
};
const DirectThirdPartyConnection = (props) => {
  const {
    openConnectWindow,
    onSelectFolder,
    isDisabled,
    isError,
    id,
    withoutInitPath,
    connectDialogVisible,
    setConnectDialogVisible,
    setDeleteThirdPartyDialogVisible,
    deleteThirdPartyDialogVisible,
    clearLocalStorage,
    setSelectedThirdPartyAccount,
    connectedThirdPartyAccount,
    selectedThirdPartyAccount,
    setConnectedThirdPartyAccount,
    buttonSize,
    isTheSameThirdPartyAccount,
    onSelectFile,
    filterParam,
    descriptionText,
  } = props;

  const { t } = useTranslation("Translations");

  useEffect(() => {
    onSetInitialInfo();

    return () => {
      setSelectedThirdPartyAccount(null);
    };
  }, []);

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

      onSelectFolder && onSelectFolder("");

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
            id: connectedAccount.id,
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
        folderList: connectedAccount ?? {},
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
    onSelectFolder && onSelectFolder("");

    const { provider_key, provider_link: directConnection } =
      selectedThirdPartyAccount;

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

  const { isLoading, folderList, isInitialLoading } = state;

  const isDisabledComponent =
    isDisabled || isInitialLoading || isLoading || accounts.length === 0;

  const isDisabledSelector = isLoading || isDisabled;

  console.log("folderList", folderList, selectedThirdPartyAccount);
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
          isDisabled={isDisabledComponent}
        />

        {connectedThirdPartyAccount?.id && isTheSameThirdPartyAccount && (
          <ContextMenuButton
            zIndex={402}
            className="backup_third-party-context"
            iconName={VerticalDotsReactSvgUrl}
            size={15}
            getData={getContextOptions}
            isDisabled={isDisabledComponent}
            displayIconBorder
          />
        )}
      </div>

      {!connectedThirdPartyAccount?.id || !isTheSameThirdPartyAccount ? (
        <Button
          id="connect-button"
          primary
          label={t("Common:Connect")}
          onClick={onConnect}
          size={buttonSize}
        />
      ) : (
        <>
          {folderList.id && (
            <FilesSelectorInput
              descriptionText={descriptionText}
              filterParam={filterParam}
              rootThirdPartyId={selectedThirdPartyAccount.id}
              onSelectFolder={onSelectFolder}
              onSelectFile={onSelectFile}
              id={id ? id : folderList.id}
              withoutInitPath={withoutInitPath}
              isError={isError}
              isDisabled={
                isLoading ||
                accounts.length === 0 ||
                folderList.length === 0 ||
                isDisabled
              }
              isThirdParty
            />
          )}
        </>
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
})(observer(DirectThirdPartyConnection));
