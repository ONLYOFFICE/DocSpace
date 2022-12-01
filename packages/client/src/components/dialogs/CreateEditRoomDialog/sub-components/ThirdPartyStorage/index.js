import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import toastr from "@docspace/components/toast/toastr";

import { StyledParam } from "../Params/StyledParam";
import ToggleParam from "../Params/ToggleParam";
import ThirdPartyComboBox from "./ThirdPartyComboBox";

import Checkbox from "@docspace/components/checkbox";
import FolderInput from "./FolderInput";
import { getOAuthToken } from "@docspace/common/utils";

const StyledThirdPartyStorage = styled(StyledParam)`
  flex-direction: column;
  gap: 12px;
`;

const ThirdPartyStorage = ({
  t,

  roomTitle,
  storageLocation,
  onChangeStorageLocation,

  setIsScrollLocked,
  setIsOauthWindowOpen,

  connectItems,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdpartyResponse,
  setSaveThirdpartyResponse,
  saveThirdParty,
  deleteThirdParty,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,

  isDisabled,
}) => {
  const onChangeIsThirdparty = () => {
    if (isDisabled) return;
    if (connectItems.length) {
      onChangeStorageLocation({
        ...storageLocation,
        isThirdparty: !storageLocation.isThirdparty,
      });
    } else {
      toastr.warning(
        <div>
          <div>{t("ThirdPartyStorageNoStorageAlert")}</div>
          <a href="#">Third-party services</a>
        </div>,
        null,
        5000,
        true,
        false
      );
    }
  };

  const onChangeProvider = async (provider) => {
    if (!!storageLocation.thirdpartyAccount) {
      onChangeStorageLocation({
        ...storageLocation,
        provider,
        thirdpartyAccount: null,
      });
      await deleteThirdParty(storageLocation.thirdpartyAccount.providerId);
      return;
    }

    onChangeStorageLocation({ ...storageLocation, provider });
  };

  const onChangeStorageFolderId = (storageFolderId) =>
    onChangeStorageLocation({
      ...storageLocation,
      storageFolderId,
    });

  const onChangeIsSaveThirdpartyAccount = () => {
    onChangeStorageLocation({
      ...storageLocation,
      rememberThirdpartyStorage: !storageLocation.rememberThirdpartyStorage,
    });
  };

  return (
    <StyledThirdPartyStorage>
      <ToggleParam
        id="shared_third-party-storage-toggle"
        title={t("ThirdPartyStorageTitle")}
        description={t("ThirdPartyStorageDescription")}
        isChecked={storageLocation.isThirdparty}
        onCheckedChange={onChangeIsThirdparty}
      />

      {storageLocation.isThirdparty && (
        <ThirdPartyComboBox
          t={t}
          storageLocation={storageLocation}
          onChangeStorageLocation={onChangeStorageLocation}
          onChangeProvider={onChangeProvider}
          connectItems={connectItems}
          setConnectDialogVisible={setConnectDialogVisible}
          setRoomCreation={setRoomCreation}
          saveThirdParty={saveThirdParty}
          saveThirdpartyResponse={saveThirdpartyResponse}
          setSaveThirdpartyResponse={setSaveThirdpartyResponse}
          openConnectWindow={openConnectWindow}
          setConnectItem={setConnectItem}
          getOAuthToken={getOAuthToken}
          setIsScrollLocked={setIsScrollLocked}
          setIsOauthWindowOpen={setIsOauthWindowOpen}
          isDisabled={isDisabled}
        />
      )}

      {storageLocation.isThirdparty && storageLocation.thirdpartyAccount && (
        <FolderInput
          t={t}
          roomTitle={roomTitle}
          thirdpartyAccount={storageLocation.thirdpartyAccount}
          onChangeStorageFolderId={onChangeStorageFolderId}
          isDisabled={isDisabled}
        />
      )}

      {/* {storageLocation.isThirdparty && storageLocation.thirdpartyAccount && (
        <Checkbox
          className="thirdparty-checkbox"
          label={t("ThirdPartyStorageRememberChoice")}
          isChecked={storageLocation.isSaveThirdpartyAccount}
          onChange={onChangeIsSaveThirdpartyAccount}
        />
      )} */}
    </StyledThirdPartyStorage>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    tagsStore,
    filesActionsStore,
    selectedFolderStore,
    settingsStore,
    dialogsStore,
  }) => {
    // const { getOAuthToken } = auth.settingsStore;
    const {
      openConnectWindow,
      saveThirdParty,
      deleteThirdParty,
    } = settingsStore.thirdPartyStore;

    const {
      setConnectItem,
      setConnectDialogVisible,
      setRoomCreation,
      saveThirdpartyResponse,
      setSaveThirdpartyResponse,
    } = dialogsStore;

    const thirdPartyStore = settingsStore.thirdPartyStore;

    const connectItems = [
      thirdPartyStore.googleConnectItem,
      thirdPartyStore.boxConnectItem,
      thirdPartyStore.dropboxConnectItem,
      thirdPartyStore.oneDriveConnectItem,
      thirdPartyStore.nextCloudConnectItem && [
        ...thirdPartyStore.nextCloudConnectItem,
        "Nextcloud",
      ],
      thirdPartyStore.kDriveConnectItem,
      thirdPartyStore.yandexConnectItem,
      thirdPartyStore.ownCloudConnectItem && [
        ...thirdPartyStore.ownCloudConnectItem,
        "ownCloud",
      ],
      thirdPartyStore.webDavConnectItem,
      thirdPartyStore.sharePointConnectItem,
    ]
      .map(
        (item) =>
          item && {
            id: item[0],
            className: `storage_${item[0].toLowerCase()}`,
            providerKey: item[0],
            isOauth: item.length > 1 && item[0] !== "WebDav",
            oauthHref: item.length > 1 && item[0] !== "WebDav" ? item[1] : "",
            ...(item[0] === "WebDav" && {
              category: item[item.length - 1],
            }),
          }
      )
      .filter((item) => !!item);

    return {
      connectItems,

      setConnectDialogVisible,
      setRoomCreation,

      saveThirdParty,
      deleteThirdParty,

      saveThirdpartyResponse,
      setSaveThirdpartyResponse,

      openConnectWindow,
      setConnectItem,
      getOAuthToken,
    };
  }
)(observer(ThirdPartyStorage));
