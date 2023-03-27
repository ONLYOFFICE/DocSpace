import React, { useState, useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { EditRoomDialog } from "../dialogs";
import { Encoder } from "@docspace/common/utils/encoder";
import api from "@docspace/common/api";
import { getRoomInfo } from "@docspace/common/api/rooms";
import toastr from "@docspace/components/toast/toastr";

const EditRoomEvent = ({
  addActiveItems,
  setActiveFolders,

  visible,
  onClose,
  item,

  editRoom,
  addTagsToRoom,
  removeTagsFromRoom,

  createTag,
  fetchTags,

  getThirdPartyIcon,

  calculateRoomLogoParams,
  uploadRoomLogo,
  setFolder,
  getFolderIndex,
  updateFolder,

  removeLogoFromRoom,
  addLogoToRoom,

  currentFolderId,
  updateCurrentFolder,
  setCreateRoomDialogVisible,

  withPaging,

  updateEditedSelectedRoom,
  addDefaultLogoPaths,
  updateLogoPathsCacheBreaker,
  removeLogoPaths,

  reloadInfoPanelSelection,
}) => {
  const { t } = useTranslation(["CreateEditRoomDialog", "Common", "Files"]);

  const [fetchedTags, setFetchedTags] = useState([]);
  const [fetchedImage, setFetchedImage] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

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
      uploadedFile: item.logo.original,
      tmpFile: "",
      x: 0.5,
      y: 0.5,
      zoom: 1,
    },
  };

  const updateRoom = (oldRoom, newRoom) => {
    // After rename of room with providerKey, it's id value changes too
    if (oldRoom.providerKey) {
      let index = getFolderIndex(oldRoom.id);

      if (index === -1) {
        index = getFolderIndex(newRoom.id);
      }

      return updateFolder(index, newRoom);
    }

    setFolder(newRoom);
  };

  const onSave = async (roomParams) => {
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

      let room = await editRoom(item.id, editRoomParams);

      room.isLogoLoading = true;

      for (let i = 0; i < newTags.length; i++) {
        await createTag(newTags[i]);
      }

      room = await addTagsToRoom(room.id, tags);
      room = await removeTagsFromRoom(room.id, removedTags);

      if (!!item.logo.original && !roomParams.icon.uploadedFile) {
        room = await removeLogoFromRoom(room.id);
      }

      if (roomParams.icon.uploadedFile) {
        updateRoom(item, {
          ...room,
          logo: { big: item.logo.small },
        });

        addActiveItems(null, [room.id]);

        const response = await uploadRoomLogo(uploadLogoData);
        const url = URL.createObjectURL(roomParams.icon.uploadedFile);
        const img = new Image();
        img.onload = async () => {
          const { x, y, zoom } = roomParams.icon;

          try {
            room = await addLogoToRoom(room.id, {
              tmpFile: response.data,
              ...calculateRoomLogoParams(img, x, y, zoom),
            });
          } catch (e) {
            toastr.error(e);
          }

          !withPaging && updateRoom(item, room);

          reloadInfoPanelSelection();
          URL.revokeObjectURL(img.src);
          setActiveFolders([]);
        };
        img.src = url;
      } else {
        !withPaging && updateRoom(item, room);
        reloadInfoPanelSelection();
      }
    } catch (err) {
      console.log(err);
    } finally {
      if (withPaging) await updateCurrentFolder(null, currentFolderId);

      if (item.id === currentFolderId) {
        updateEditedSelectedRoom(editRoomParams.title, tags);
        if (item.logo.original && !roomParams.icon.uploadedFile) {
          removeLogoPaths();
          reloadInfoPanelSelection();
        } else if (!item.logo.original && roomParams.icon.uploadedFile)
          addDefaultLogoPaths();
        else if (item.logo.original && roomParams.icon.uploadedFile)
          updateLogoPathsCacheBreaker();
      }

      setIsLoading(false);
      onClose();
    }
  };

  const fetchLogoAction = useCallback(async (logo) => {
    const imgExst = logo.slice(".")[1];
    const file = await fetch(logo)
      .then((res) => res.arrayBuffer())
      .then(
        (buf) =>
          new File([buf], "fetchedFile", {
            type: `image/${imgExst}`,
          })
      );
    setFetchedImage(file);
  }, []);

  useEffect(() => {
    const logo = item?.logo?.original ? item.logo.original : "";

    if (logo) {
      fetchLogoAction(logo);
    }
  }, []);

  const fetchTagsAction = useCallback(async () => {
    const tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

  useEffect(() => {
    fetchTagsAction();
  }, [fetchTagsAction]);

  useEffect(() => {
    setCreateRoomDialogVisible(true);
    return () => setCreateRoomDialogVisible(false);
  }, []);

  return (
    <EditRoomDialog
      t={t}
      visible={visible}
      onClose={onClose}
      fetchedRoomParams={fetchedRoomParams}
      onSave={onSave}
      fetchedTags={fetchedTags}
      fetchedImage={fetchedImage}
      isLoading={isLoading}
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
    dialogsStore,
    settingsStore,
  }) => {
    const {
      editRoom,
      addTagsToRoom,
      removeTagsFromRoom,
      calculateRoomLogoParams,
      uploadRoomLogo,
      setFolder,
      getFolderIndex,
      updateFolder,
      addLogoToRoom,
      removeLogoFromRoom,
      addActiveItems,
      setActiveFolders,
    } = filesStore;

    const { createTag, fetchTags } = tagsStore;
    const {
      id: currentFolderId,
      updateEditedSelectedRoom,
      addDefaultLogoPaths,
      removeLogoPaths,
      updateLogoPathsCacheBreaker,
    } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;
    const { getThirdPartyIcon } = settingsStore.thirdPartyStore;
    const { setCreateRoomDialogVisible } = dialogsStore;
    const { withPaging } = auth.settingsStore;
    const { reloadSelection: reloadInfoPanelSelection } = auth.infoPanelStore;
    return {
      addActiveItems,
      setActiveFolders,

      editRoom,
      addTagsToRoom,
      removeTagsFromRoom,

      createTag,
      fetchTags,

      getThirdPartyIcon,

      calculateRoomLogoParams,
      setFolder,
      getFolderIndex,
      updateFolder,
      uploadRoomLogo,
      removeLogoFromRoom,
      addLogoToRoom,

      currentFolderId,
      updateCurrentFolder,

      withPaging,
      setCreateRoomDialogVisible,

      updateEditedSelectedRoom,
      addDefaultLogoPaths,
      updateLogoPathsCacheBreaker,
      removeLogoPaths,

      reloadInfoPanelSelection,
    };
  }
)(observer(EditRoomEvent));
