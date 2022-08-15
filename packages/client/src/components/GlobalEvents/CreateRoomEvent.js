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
  uploadRoomLogo,
  addLogoToRoom,
  fetchTags,

  providers,

  currrentFolderId,
  updateCurrentFolder,
}) => {
  const { t } = useTranslation([
    "CreateEditRoomDialog",
    "Common",
    "Files",
    "ToastHeaders",
  ]);
  const [fetchedTags, setFetchedTags] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  console.log(providers);

  const onCreate = async (roomParams) => {
    console.log(roomParams);

    const createRoomData = {
      roomType: roomParams.type,
      title: roomParams.title || t("Files:NewRoom"),
    };

    const addTagsData = roomParams.tags.map((tag) => tag.name);

    const createTagsData = roomParams.tags
      .filter((t) => t.isNew)
      .map((t) => t.name);

    const uploadLogoData = new FormData();
    uploadLogoData.append(0, roomParams.icon.uploadedFile);

    try {
      setIsLoading(true);

      const room = await createRoom(createRoomData);

      for (let i = 0; i < createTagsData.length; i++)
        await createTag(createTagsData[i]);

      await addTagsToRoom(room.id, addTagsData);

      if (roomParams.icon.uploadedFile)
        await uploadRoomLogo(uploadLogoData).then((response) => {
          const { x, y, width, height } = roomParams.icon;
          addLogoToRoom({ tmpFile: response.data, x, y, width, height });
        });

      await updateCurrentFolder(null, currrentFolderId);
    } catch (err) {
      console.log(err);
    } finally {
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
      visible={visible}
      onClose={onClose}
      onCreate={onCreate}
      fetchedTags={fetchedTags}
      isLoading={isLoading}
      providers={providers}
    />
  );
};

export default inject(
  ({
    filesStore,
    tagsStore,
    filesActionsStore,
    selectedFolderStore,
    settingsStore,
  }) => {
    const {
      createRoom,
      createRoomInThirdpary,
      addTagsToRoom,
      uploadRoomLogo,
      addLogoToRoom,
    } = filesStore;
    const { createTag, fetchTags } = tagsStore;

    const { id: currrentFolderId } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;

    const { providers } = settingsStore.thirdPartyStore;

    return {
      createRoom,
      createRoomInThirdpary,
      createTag,
      fetchTags,
      addTagsToRoom,
      uploadRoomLogo,
      addLogoToRoom,

      providers,

      currrentFolderId,
      updateCurrentFolder,
    };
  }
)(observer(CreateRoomEvent));
