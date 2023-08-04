import React, { useState, useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { CreateRoomDialog } from "../dialogs";
import { isMobile } from "react-device-detect";

const CreateRoomEvent = ({
  visible,
  onClose,

  fetchTags,
  setRoomParams,
  onCreateRoom,
  createRoomConfirmDialogVisible,
  setCreateRoomConfirmDialogVisible,
  confirmDialogIsLoading,
  connectDialogVisible,

  isLoading,
  setIsLoading,
  setOnClose,
  setCreateRoomDialogVisible,

  fetchThirdPartyProviders,
  enableThirdParty,
  deleteThirdParty,
}) => {
  const { t } = useTranslation(["CreateEditRoomDialog", "Common", "Files"]);
  const [fetchedTags, setFetchedTags] = useState([]);

  const onCreate = (roomParams) => {
    setRoomParams(roomParams);
    setOnClose(onClose);

    if (
      roomParams.storageLocation.isThirdparty &&
      !roomParams.storageLocation.storageFolderId
    ) {
      setCreateRoomConfirmDialogVisible(true);
    } else {
      onCreateRoom();
    }
  };

  const fetchTagsAction = useCallback(async () => {
    let tags = await fetchTags();
    setFetchedTags(tags);
  }, []);

  useEffect(() => {
    fetchTagsAction;
  }, []);

  useEffect(() => {
    setCreateRoomDialogVisible(true);
    return () => setCreateRoomDialogVisible(false);
  }, []);

  return (
    <CreateRoomDialog
      t={t}
      visible={
        visible &&
        !connectDialogVisible &&
        !createRoomConfirmDialogVisible &&
        !confirmDialogIsLoading
      }
      onClose={onClose}
      onCreate={onCreate}
      fetchedTags={fetchedTags}
      isLoading={isLoading}
      setIsLoading={setIsLoading}
      deleteThirdParty={deleteThirdParty}
      fetchThirdPartyProviders={fetchThirdPartyProviders}
      enableThirdParty={enableThirdParty}
    />
  );
};

export default inject(
  ({
    createEditRoomStore,

    tagsStore,
    dialogsStore,
    settingsStore,
  }) => {
    const { fetchTags } = tagsStore;

    const { deleteThirdParty, fetchThirdPartyProviders } =
      settingsStore.thirdPartyStore;
    const { enableThirdParty } = settingsStore;

    const {
      createRoomConfirmDialogVisible,
      setCreateRoomConfirmDialogVisible,
      connectDialogVisible,
      setCreateRoomDialogVisible,
    } = dialogsStore;

    const {
      setRoomParams,
      onCreateRoom,
      isLoading,
      setIsLoading,
      setOnClose,
      confirmDialogIsLoading,
    } = createEditRoomStore;

    return {
      fetchTags,
      setRoomParams,
      onCreateRoom,
      createRoomConfirmDialogVisible,
      setCreateRoomConfirmDialogVisible,
      connectDialogVisible,
      isLoading,
      setIsLoading,
      setOnClose,
      confirmDialogIsLoading,
      setCreateRoomDialogVisible,
      fetchThirdPartyProviders,
      enableThirdParty,
      deleteThirdParty,
    };
  }
)(observer(CreateRoomEvent));
