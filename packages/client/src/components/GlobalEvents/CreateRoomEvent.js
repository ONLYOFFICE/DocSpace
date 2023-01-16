import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { CreateRoomDialog } from "../dialogs";
import { toastr } from "@docspace/components";
import { isMobile } from "react-device-detect";

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

  currentFolderId,
  updateCurrentFolder,

  withPaging,
  setCreateRoomDialogVisible,
  fetchFiles,
  setInfoPanelIsVisible,
  setView,
  isAdmin,
}) => {
  const { t } = useTranslation(["CreateEditRoomDialog", "Common", "Files"]);
  const [fetchedTags, setFetchedTags] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const openNewRoom = (id) => {
    setView("info_members");
    fetchFiles(id)
      .then(() => {
        !isMobile && setInfoPanelIsVisible(true);
      })
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  };

  const onCreate = async (roomParams) => {
    const createRoomData = {
      roomType: roomParams.type,
      title: roomParams.title || t("Files:NewRoom"),
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
      let room =
        isThirdparty && storageFolderId
          ? await createRoomInThirdpary(storageFolderId, createRoomData)
          : await createRoom(createRoomData);

      room.isLogoLoading = true;

      // delete thirdparty account if not needed
      if (!isThirdparty && storageFolderId)
        await deleteThirdParty(thirdpartyAccount.providerId);

      // create new tags
      for (let i = 0; i < createTagsData.length; i++)
        await createTag(createTagsData[i]);

      // add new tags to room
      if (!!addTagsData.length)
        room = await addTagsToRoom(room.id, addTagsData);

      // calculate and upload logo to room
      if (roomParams.icon.uploadedFile) {
        await uploadRoomLogo(uploadLogoData).then((response) => {
          const url = URL.createObjectURL(roomParams.icon.uploadedFile);
          const img = new Image();
          img.onload = async () => {
            const { x, y, zoom } = roomParams.icon;
            room = await addLogoToRoom(room.id, {
              tmpFile: response.data,
              ...calculateRoomLogoParams(img, x, y, zoom),
            });

            !withPaging && openNewRoom(room.id);

            URL.revokeObjectURL(img.src);
          };
          img.src = url;
        });
      } else !withPaging && openNewRoom(room.id);
    } catch (err) {
      toastr.error(err);
      console.log(err);

      setIsLoading(false);
      onClose();
    } finally {
      if (withPaging) {
        await updateCurrentFolder(null, currentFolderId);
      }
    }
  };

  useEffect(async () => {
    let tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

  useEffect(() => {
    setCreateRoomDialogVisible(true);

    return () => setCreateRoomDialogVisible(false);
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
      isAdmin={isAdmin}
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
      createRoom,
      createRoomInThirdpary,
      addTagsToRoom,
      calculateRoomLogoParams,
      uploadRoomLogo,
      addLogoToRoom,
      fetchFiles,
      addItem,
    } = filesStore;
    const { createTag, fetchTags } = tagsStore;

    const { id: currentFolderId } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;

    const { connectDialogVisible, setCreateRoomDialogVisible } = dialogsStore;

    const {
      deleteThirdParty,
      fetchThirdPartyProviders,
    } = settingsStore.thirdPartyStore;
    const { withPaging } = auth.settingsStore;

    const {
      setIsVisible: setInfoPanelIsVisible,
      setView,
    } = auth.infoPanelStore;

    const { isAdmin } = auth;

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
      currentFolderId,
      updateCurrentFolder,

      withPaging,
      setCreateRoomDialogVisible,
      fetchFiles,
      setInfoPanelIsVisible,
      setView,
      isAdmin,
    };
  }
)(observer(CreateRoomEvent));
