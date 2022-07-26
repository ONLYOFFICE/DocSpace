import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import toastr from "studio/toastr";
import { CreateRoomDialog } from "../dialogs";
import { addTagsToRoom } from "@appserver/common/api/rooms";

const CreateRoomEvent = ({
  onClose,

  createRoom,
  createTag,
  addTagsToRoom,
  fetchTags,

  currrentFolderId,
  updateCurrentFolder,

  folderFormValidation,
}) => {
  const { t } = useTranslation([
    "CreateRoomDialog",
    "Common",
    "Settings",
    "Home",
  ]);
  const [fetchedTags, setFetchedTags] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const onCreate = async (roomParams) => {
    console.log(roomParams);
    const createRoomParams = {
      roomType: roomParams.type,
      title: roomParams.title || "New room",
    };

    try {
      setIsLoading(true);
      const tags = roomParams.tags.map((tag) => tag.name);
      const newTags = roomParams.tags
        .filter((tag) => tag.isNew)
        .map((tag) => tag.name);
      console.log(tags, newTags);

      const room = await createRoom(createRoomParams);
      console.log(room);
      for (let i = 0; i < newTags.length; i++) await createTag(newTags[i]);
      await addTagsToRoom(room.id, tags);
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
      visible={true}
      onClose={onClose}
      onCreate={onCreate}
      fetchedTags={fetchedTags}
      isLoading={isLoading}
    />
  );
};

export default inject(
  ({ auth, filesStore, tagsStore, filesActionsStore, selectedFolderStore }) => {
    const { createRoom, addTagsToRoom } = filesStore;
    const { createTag, fetchTags } = tagsStore;

    const { id } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;

    const { folderFormValidation } = auth.settingsStore;

    return {
      createRoom,
      createTag,
      fetchTags,
      addTagsToRoom,
      currrentFolderId: id,
      updateCurrentFolder,
      folderFormValidation,
    };
  }
)(observer(CreateRoomEvent));
