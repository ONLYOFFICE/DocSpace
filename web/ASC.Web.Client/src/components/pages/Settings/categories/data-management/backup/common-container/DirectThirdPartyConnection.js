import React, { useEffect, useState } from "react";
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

  const updateAccountsInfo = async () => {
    try {
      setIsLoading(true);
      const connectedAccount = await getConnectedAccounts();
      onSetThirdPartySettings(
        connectedAccount?.providerKey,
        connectedAccount?.providerId,
        connectedAccount
      );
    } catch (e) {
      setIsLoading(false);
      if (!e) return;
      toastr.error(e);
    }
  };
  const onSetThirdPartySettings = async (providerKey, providerId, account) => {
    try {
      !isLoading && setIsLoading(true);

      const capabilities = await getThirdPartyCapabilities();
      accounts = [];
      let connectedAccount = {};

      for (let i = 0; i < capabilities.length; i++) {
        const isConnected = capabilities[i][0] === providerKey;

        if (capabilities[i][0] !== "WebDav") {
          accounts.push({
            key: i.toString(),
            label: capabilities[i][0],
            provider_key: capabilities[i][0],
            ...(capabilities[i][1] && { provider_link: capabilities[i][1] }),
            connected: isConnected,
            ...(isConnected && {
              provider_id: providerId,
            }),
          });

          if (isConnected) {
            connectedAccount = { ...accounts[i] };
          }
        } else {
          const WebDavVariability = [
            "Nextcloud",
            "ownCloud",
            "ConnextOtherAccount",
          ];
          let index = 0;

          for (let j = i; j < i + 3; j++) {
            accounts.push({
              key: j.toString(),
              label: WebDavVariability[index],
              provider_key: "WebDav",
              connected: isConnected,
              ...(isConnected && {
                provider_id: providerId,
              }),
            });

            // if (isConnected) { //TODO: changed for WebDav
            //   connectedAccount = { ...accounts[j] };
            // }

            index++;
          }
        }
      }
      setSelectedAccount(
        Object.keys(connectedAccount).length !== 0
          ? connectedAccount
          : { ...accounts[0] }
      );
      setFolderList(account ? account : []);
    } catch (e) {
      if (!e) return;
      toastr.error(e);
    }

    setIsLoading(false);
  };

  const [selectedAccount, setSelectedAccount] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [isVisibleConnectionForm, setIsVisibleConnectionForm] = useState(false);
  const [folderList, setFolderList] = useState([]);
  const onConnect = () => {
    const { label, provider_link } = selectedAccount;

    const directConnection = provider_link;

    if (directConnection) {
      let authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );

      openConnectWindow(label, authModal)
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
    const { label, provider_key, provider_id } = selectedAccount;

    try {
      setIsLoading(true);

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
      isVisibleConnectionForm && setIsVisibleConnectionForm(false);
    } catch (e) {
      setIsLoading(false);
      if (!e) return;
      toastr.error(e);
    }
  };

  const onCloseConnectionForm = () => {
    setIsVisibleConnectionForm(false);
  };

  const onSelectAccount = (options) => {
    const key = options.key;

    setSelectedAccount({ ...accounts[+key] });
  };

  const onReconnect = () => {
    let authModal = window.open("", "Authorization", "height=600, width=1020");
    openConnectWindow(selectedAccount.provider_key, authModal).then((modal) =>
      getOAuthToken(modal).then((token) => {
        authModal.close();
        saveSettings(token);
      })
    );
  };

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
