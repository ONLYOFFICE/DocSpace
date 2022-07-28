import React, { useEffect, useState, useReducer } from "react";
import Button from "@appserver/components/button";
import SelectFolderInput from "files/SelectFolderInput";
import {
  getSettingsThirdParty,
  getThirdPartyCapabilities,
  saveSettingsThirdParty,
} from "@appserver/common/api/files";
import { StyledBackup } from "../StyledBackup";
import Text from "@appserver/components/text";
import ComboBox from "@appserver/components/combobox";
import toastr from "@appserver/components/toast/toastr";
import FormConnection from "./FormConnection";
import { inject, observer } from "mobx-react";

let accounts = [];
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
    withoutBasicSelection,
    isError,
    id,
    isReset,
    isSuccessSave,
  } = props;

  useEffect(() => {
    updateAccountsInfo();
  }, []);

  const [isVisibleConnectionForm, setIsVisibleConnectionForm] = useState(false);
  const initialState = {
    selectedAccount: {},
    folderList: [],
    isLoading: true,
  };

  const [state, setState] = useReducer(
    (state, newState) => ({ ...state, ...newState }),
    initialState
  );

  const updateAccountsInfo = async () => {
    try {
      const connectedAccount = await getConnectedAccounts();
      onSetThirdPartySettings(
        connectedAccount?.providerKey,
        connectedAccount?.providerId,
        connectedAccount
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
    account
  ) => {
    try {
      !state.isLoading && setState({ isLoading: true });

      const capabilities = await getThirdPartyCapabilities();
      accounts = [];

      let index = 0,
        connectedAccount = {};

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
          connectedAccount = { ...accounts[index] };
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
        selectedAccount:
          Object.keys(connectedAccount).length !== 0
            ? connectedAccount
            : { ...accounts[0] },
        folderList: account ? account : [],
      });
    } catch (e) {
      setState({ isLoading: false });
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

  const { selectedAccount, isLoading, folderList } = state;
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
          isDisabled={isDisabled || isLoading || accounts.length === 0}
        />

        {selectedAccount.connected ? (
          <Button
            label={t("Reconnect")}
            onClick={onReconnect}
            size={"small"}
            isDisabled={isDisabled || isLoading || accounts.length === 0}
          />
        ) : (
          <Button
            isDisabled={isDisabled || isLoading || accounts.length === 0}
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
        foldersType="third-party"
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
