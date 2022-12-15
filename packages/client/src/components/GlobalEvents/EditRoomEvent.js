import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { EditRoomDialog } from "../dialogs";
import { Encoder } from "@docspace/common/utils/encoder";
import api from "@docspace/common/api";

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

  calculateRoomLogoParams,
  uploadRoomLogo,
  setFolder,
  removeLogoFromRoom,
  addLogoToRoom,

  currentFolderId,
  updateCurrentFolder,
  setCreateRoomDialogVisible,

  withPaging,

  reloadSelection,
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

      for (let i = 0; i < newTags.length; i++) await createTag(newTags[i]);
      room = await addTagsToRoom(room.id, tags);
      room = await removeTagsFromRoom(room.id, removedTags);

      if (!!item.logo.original && !roomParams.icon.uploadedFile)
        room = await removeLogoFromRoom(room.id);

      if (roomParams.icon.uploadedFile) {
        await setFolder({
          ...room,
          logo: { big: item.logo.small },
        });

        await uploadRoomLogo(uploadLogoData).then((response) => {
          const url = URL.createObjectURL(roomParams.icon.uploadedFile);
          const img = new Image();
          img.onload = async () => {
            const { x, y, zoom } = roomParams.icon;
            room = await addLogoToRoom(room.id, {
              tmpFile: response.data,
              ...calculateRoomLogoParams(img, x, y, zoom),
            });

            if (!withPaging) {
              setFolder(room);
            }

            // to update state info panel
            reloadSelection();

            URL.revokeObjectURL(img.src);
          };
          img.src = url;
        });
      } else {
        if (!withPaging) {
          setFolder(room);
        }
        // to update state info panel
        reloadSelection();
      }
    } catch (err) {
      console.log(err);
    } finally {
      if (withPaging) {
        await updateCurrentFolder(null, currentFolderId);
      }
      setIsLoading(false);

      onClose();
    }
  };

  useEffect(async () => {
    const logo = item?.logo?.original ? item.logo.original : "";

    if (logo) {
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
    }
  }, []);

  useEffect(async () => {
    const tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

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
      addLogoToRoom,
      removeLogoFromRoom,
    } = filesStore;

    const { createTag, fetchTags } = tagsStore;
    const { id: currentFolderId } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;
    const { getThirdPartyIcon } = settingsStore.thirdPartyStore;
    const { setCreateRoomDialogVisible } = dialogsStore;
    const { withPaging } = auth.settingsStore;
    const { reloadSelection } = auth.infoPanelStore;
    return {
      editRoom,
      addTagsToRoom,
      removeTagsFromRoom,

      createTag,
      fetchTags,

      getThirdPartyIcon,

      calculateRoomLogoParams,
      setFolder,
      uploadRoomLogo,
      removeLogoFromRoom,
      addLogoToRoom,

      currentFolderId,
      updateCurrentFolder,

      withPaging,
      setCreateRoomDialogVisible,

      reloadSelection,
    };
  }
)(observer(EditRoomEvent));
