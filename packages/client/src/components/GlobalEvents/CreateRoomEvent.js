import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { CreateRoomDialog } from "../dialogs";
import { toastr } from "@docspace/components";

const CreateRoomEvent = ({
  visible,
  onClose,

  createRoom,
  createRoomInThirdpary,
  createTag,
  addTagsToRoom,
  deleteThirdParty,
  fetchThirdPartyProviders,
  calculateRoomLogoParams,
  uploadRoomLogo,
  addLogoToRoom,
  fetchTags,

  connectDialogVisible,

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
      private: roomParams.isPrivate,
    };

    const createTagsData = roomParams.tags
      .filter((t) => t.isNew)
      .map((t) => t.name);
    const addTagsData = roomParams.tags.map((tag) => tag.name);

    const isThirdparty = roomParams.storageLocation.isThirdparty;
    const storageFolderId = roomParams.storageLocation.storageFolderId;
    const thirdpartyAccount = roomParams.storageLocation.thirdpartyAccount;

    const uploadLogoData = new FormData();
    uploadLogoData.append(0, roomParams.icon.uploadedFile);

    try {
      setIsLoading(true);

      // create room
      const room =
        isThirdparty && storageFolderId
          ? await createRoomInThirdpary(storageFolderId, createRoomData)
          : await createRoom(createRoomData);

      // delete thirdparty account if not needed
      if (!isThirdparty && storageFolderId)
        await deleteThirdParty(thirdpartyAccount.providerId);

      // create new tags
      for (let i = 0; i < createTagsData.length; i++)
        await createTag(createTagsData[i]);

      // add new tags to room
      await addTagsToRoom(room.id, addTagsData);

      // calculate and upload logo to room
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
      toastr.error(err);
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
      setIsLoading={setIsLoading}
      deleteThirdParty={deleteThirdParty}
      fetchThirdPartyProviders={fetchThirdPartyProviders}
    />
  );
};

export default inject(
  ({
    filesStore,
    tagsStore,
    filesActionsStore,
    selectedFolderStore,
    dialogsStore,
    settingsStore,
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

    const { connectDialogVisible } = dialogsStore;

    const {
      deleteThirdParty,
      fetchThirdPartyProviders,
    } = settingsStore.thirdPartyStore;

    return {
      createRoom,
      createRoomInThirdpary,
      createTag,
      fetchTags,
      addTagsToRoom,
      deleteThirdParty,
      fetchThirdPartyProviders,
      calculateRoomLogoParams,
      uploadRoomLogo,
      addLogoToRoom,

      connectDialogVisible,
      currrentFolderId,
      updateCurrentFolder,
    };
  }
)(observer(CreateRoomEvent));
