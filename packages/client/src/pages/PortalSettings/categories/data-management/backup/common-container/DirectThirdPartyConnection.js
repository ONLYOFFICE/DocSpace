import React, { useEffect, useState, useReducer } from "react";
import Button from "@docspace/components/button";
import SelectFolderInput from "client/SelectFolderInput";
import {
  getSettingsThirdParty,
  getThirdPartyCapabilities,
  saveSettingsThirdParty,
} from "@docspace/common/api/files";
import { StyledBackup } from "../StyledBackup";
import Text from "@docspace/components/text";
import ComboBox from "@docspace/components/combobox";
import toastr from "@docspace/components/toast/toastr";
import FormConnection from "./FormConnection";
import { inject, observer } from "mobx-react";

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
    isReset,
    isSuccessSave,
    withoutBasicSelection,
  } = props;

  useEffect(() => {
    updateAccountsInfo();
  }, []);

  const [isVisibleConnectionForm, setIsVisibleConnectionForm] = useState(false);
  const initialState = {
    selectedAccount: {},
    folderList: [],
    isLoading: false,
    isInitialLoading: true,
  };

  const [state, setState] = useReducer(
    (state, newState) => ({ ...state, ...newState }),
    initialState
  );

  const updateAccountsInfo = async () => {
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

      //TODO: need to use connectedCloudsTypeTitleTranslation for title
      setAccount("GoogleDrive", "Google Drive");
      setAccount("Box", "Box");
      setAccount("DropboxV2", "DropboxV2");
      setAccount("SharePoint", "SharePoint");
      setAccount("OneDrive", "OneDrive");
      setAccount("WebDav", "Nextcloud");
      setAccount("WebDav", "ownCloud");
      setAccount("kDrive", "kDrive");
      setAccount("Yandex", "Yandex.Disk");
      setAccount("WebDav", "WebDAV");

      setState({
        isLoading: false,
        isInitialLoading: false,
        selectedAccount:
          Object.keys(selectedAccount).length !== 0
            ? selectedAccount
            : { ...accounts[0] },
        folderList: account ? account : [],
      });
    } catch (e) {
      setState({ isLoading: false, isInitialLoading: false });
      if (!e) return;
      toastr.error(e);
    }
  };

  const onConnect = () => {
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
      setIsVisibleConnectionForm(true);
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
    setState({ isLoading: true });
    isVisibleConnectionForm && setIsVisibleConnectionForm(false);
    onSelectFolder("");

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
      setState({ isLoading: false });
      toastr.error(e);
    }
  };

  const onCloseConnectionForm = () => {
    setIsVisibleConnectionForm(false);
  };

  const onSelectAccount = (options) => {
    const key = options.key;

    setState({ selectedAccount: { ...accounts[+key] } });
  };

  const onReconnect = () => {
    const { provider_link } = state.selectedAccount;

    const directConnection = provider_link;
    console.log("selectedAccount", selectedAccount);
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
      setIsVisibleConnectionForm(true);
    }
  };

  const { selectedAccount, isLoading, folderList, isInitialLoading } = state;

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
            isDisabled || isInitialLoading || isLoading || accounts.length === 0
          }
        />

        {selectedAccount.connected ? (
          <Button
            label={t("Reconnect")}
            onClick={onReconnect}
            size={"small"}
            isDisabled={
              isDisabled ||
              isInitialLoading ||
              isLoading ||
              accounts.length === 0
            }
          />
        ) : (
          <Button
            isDisabled={
              isDisabled ||
              isInitialLoading ||
              isLoading ||
              accounts.length === 0
            }
            label={t("Common:Connect")}
            onClick={onConnect}
            size={"small"}
          />
        )}
      </div>
      <Text className="backup_third-party-text" fontWeight={"600"}>
        {"Folder name:"}
      </Text>

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
        foldersType={isInitialLoading ? "" : "third-party"}
        foldersList={[folderList]}
        withoutBasicSelection={withoutBasicSelection}
        isReset={isReset}
        isSuccessSave={isSuccessSave}
      />

      {isVisibleConnectionForm && (
        <FormConnection
          t={t}
          saveSettings={saveSettings}
          item={selectedAccount}
          visible={isVisibleConnectionForm}
          onClose={onCloseConnectionForm}
        />
      )}
    </StyledBackup>
  );
};

export default inject(({ backup }) => {
  const { commonThirdPartyList, openConnectWindow, getOAuthToken } = backup;

  return {
    commonThirdPartyList,
    openConnectWindow,
    getOAuthToken,
  };
})(observer(DirectThirdPartyConnection));
