import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { EditRoomDialog } from "../dialogs";

const EditRoomEvent = ({
  visible,
  onClose,
  item,

  editRoom,
  addTagsToRoom,
  removeTagsFromRoom,

  createTag,
  fetchTags,

  getThirdPartyIcon,

  uploadRoomLogo,
  addLogoToRoom,

  currrentFolderId,
  updateCurrentFolder,
}) => {
  const { t } = useTranslation(["CreateEditRoomDialog", "Common", "Files"]);

  const [fetchedTags, setFetchedTags] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const roomId = item.id;
  const startTags = Object.values(item.tags);
  const startObjTags = startTags.map((tag, i) => ({ id: i, name: tag }));

  const fetchedRoomParams = {
    title: item.title,
    type: item.roomType,
    tags: startObjTags,
    isThirdparty: !!item.providerKey,
    storageLocation: {
      title: item.title,
      parentId: item.parentId,
      providerKey: item.providerKey,
      iconSrc: getThirdPartyIcon(item.providerKey),
    },
    isPrivate: false,
    icon: {
      uploadedFileSrc: item.icon,
      tmpFile: "",
      x: 0.5,
      y: 0.5,
      width: 216,
      height: 216,
      zoom: 1,
    },
  };

  const onSave = async (roomParams) => {
    console.log(roomParams);

    const editRoomParams = {
      title: roomParams.title || t("Files:NewRoom"),
    };

    const tags = roomParams.tags.map((tag) => tag.name);
    const newTags = roomParams.tags.filter((t) => t.isNew).map((t) => t.name);
    const removedTags = startTags.filter((sT) => !tags.includes(sT));

    const uploadLogoData = new FormData();
    uploadLogoData.append(0, roomParams.icon.uploadedFile);

    try {
      setIsLoading(true);

      await editRoom(roomId, editRoomParams);
      for (let i = 0; i < newTags.length; i++) await createTag(newTags[i]);
      await addTagsToRoom(roomId, tags);
      await removeTagsFromRoom(roomId, removedTags);

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
    const tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

  console.log(item);

  return (
    <EditRoomDialog
      t={t}
      visible={visible}
      onClose={onClose}
      fetchedRoomParams={fetchedRoomParams}
      onSave={onSave}
      fetchedTags={fetchedTags}
      isLoading={isLoading}
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
      editRoom,
      addTagsToRoom,
      removeTagsFromRoom,
      uploadRoomLogo,
      addLogoToRoom,
    } = filesStore;

    const { createTag, fetchTags } = tagsStore;
    const { id: currrentFolderId } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;
    const { getThirdPartyIcon } = settingsStore.thirdPartyStore;

    return {
      editRoom,
      addTagsToRoom,
      removeTagsFromRoom,

      createTag,
      fetchTags,

      getThirdPartyIcon,

      uploadRoomLogo,
      addLogoToRoom,

      currrentFolderId,
      updateCurrentFolder,
    };
  }
)(observer(EditRoomEvent));
