import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { CreateRoomDialog } from "../dialogs";

const CreateRoomEvent = ({
  visible,
  onClose,

  createRoom,
  createRoomInThirdpary,
  createTag,
  addTagsToRoom,
  calculateRoomLogoParams,
  uploadRoomLogo,
  addLogoToRoom,
  fetchTags,

  connectItems,
  connectDialogVisible,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdpartyResponse,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,

  currrentFolderId,
  updateCurrentFolder,
}) => {
  const { t } = useTranslation(["CreateEditRoomDialog", "Common", "Files"]);
  const [fetchedTags, setFetchedTags] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const onCreate = async (roomParams) => {
    const createRoomData = {
      roomType: roomParams.type,
      title: roomParams.title || t("Files:NewRoom"),
    };

    const isThirdparty =
      roomParams.isThirdparty &&
      roomParams.storageLocation.isConnected &&
      roomParams.storageLocation.thirdpartyFolderId;

    const addTagsData = roomParams.tags.map((tag) => tag.name);

    const createTagsData = roomParams.tags
      .filter((t) => t.isNew)
      .map((t) => t.name);

    const uploadLogoData = new FormData();
    uploadLogoData.append(0, roomParams.icon.uploadedFile);

    try {
      setIsLoading(true);

      const room = isThirdparty
        ? await createRoomInThirdpary(
            roomParams.storageLocation.thirdpartyFolderId,
            createRoomData
          )
        : await createRoom(createRoomData);

      for (let i = 0; i < createTagsData.length; i++)
        await createTag(createTagsData[i]);

      await addTagsToRoom(room.id, addTagsData);

      if (roomParams.icon.uploadedFile)
        await uploadRoomLogo(uploadLogoData).then((response) => {
          const url = URL.createObjectURL(roomParams.icon.uploadedFile);
          const img = new Image();
          img.onload = async () => {
            const { x, y, zoom } = roomParams.icon;
            await addLogoToRoom(room.id, {
              tmpFile: response.data,
              ...calculateRoomLogoParams(img, x, y, zoom),
            });
            URL.revokeObjectURL(img.src);
          };
          img.src = url;
        });
    } catch (err) {
      console.log(err);
    } finally {
      await updateCurrentFolder(null, currrentFolderId);
      setIsLoading(false);
      onClose();
    }
  };

  useEffect(async () => {
    let tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

  return (
    <CreateRoomDialog
      t={t}
      visible={visible && !connectDialogVisible}
      onClose={onClose}
      onCreate={onCreate}
      fetchedTags={fetchedTags}
      isLoading={isLoading}
      connectItems={connectItems}
      connectDialogVisible={connectDialogVisible}
      setConnectDialogVisible={setConnectDialogVisible}
      setRoomCreation={setRoomCreation}
      saveThirdpartyResponse={saveThirdpartyResponse}
      openConnectWindow={openConnectWindow}
      setConnectItem={setConnectItem}
      getOAuthToken={getOAuthToken}
    />
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
    const {
      createRoom,
      createRoomInThirdpary,
      addTagsToRoom,
      calculateRoomLogoParams,
      uploadRoomLogo,
      addLogoToRoom,
    } = filesStore;
    const { createTag, fetchTags } = tagsStore;

    const { id: currrentFolderId } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;

    const thirdPartyStore = settingsStore.thirdPartyStore;

    const { openConnectWindow } = settingsStore.thirdPartyStore;

    const connectItems = [
      thirdPartyStore.googleConnectItem,
      thirdPartyStore.boxConnectItem,
      thirdPartyStore.dropboxConnectItem,
      thirdPartyStore.oneDriveConnectItem,
      thirdPartyStore.nextCloudConnectItem,
      thirdPartyStore.kDriveConnectItem,
      thirdPartyStore.yandexConnectItem,
      thirdPartyStore.ownCloudConnectItem,
      thirdPartyStore.webDavConnectItem,
      thirdPartyStore.sharePointConnectItem,
    ]
      .map(
        (item) =>
          item && {
            isAvialable: !!item,
            id: item[0],
            providerName: item[0],
            isOauth: item.length > 1,
            oauthHref: item.length > 1 ? item[1] : "",
          }
      )
      .filter((item) => !!item);

    const { getOAuthToken } = auth.settingsStore;

    const {
      setConnectItem,
      connectDialogVisible,
      setConnectDialogVisible,
      setRoomCreation,
      saveThirdpartyResponse,
    } = dialogsStore;

    return {
      createRoom,
      createRoomInThirdpary,
      createTag,
      fetchTags,
      addTagsToRoom,
      calculateRoomLogoParams,
      uploadRoomLogo,
      addLogoToRoom,

      setConnectItem,
      connectDialogVisible,
      setConnectDialogVisible,
      setRoomCreation,
      saveThirdpartyResponse,
      saveThirdpartyResponse,
      openConnectWindow,
      connectItems,
      getOAuthToken,

      currrentFolderId,
      updateCurrentFolder,
    };
  }
)(observer(CreateRoomEvent));
