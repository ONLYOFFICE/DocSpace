import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import toastr from "studio/toastr";
import { CreateRoomDialog } from "../dialogs";
import { addTagsToRoom } from "@appserver/common/api/rooms";

const CreateRoomEvent = ({
  createRoom,
  createTag,
  addTagsToRoom,
  fetchTags,
  onClose,

  currrentFolderId,
  updateCurrentFolder,
}) => {
  const { t } = useTranslation(["CreateRoomDialog", "Translations", "Common"]);
  const [fetchedTags, setFetchedTags] = useState([]);

  const onCreate = async (roomParams) => {
    console.log(roomParams);
    const createRoomParams = {
      roomType: roomParams.type,
      title: roomParams.title || "New room",
    };

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
    onClose();
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
    />
  );
};

export default inject(
  ({ filesStore, tagsStore, filesActionsStore, selectedFolderStore }) => {
    const { createRoom, addTagsToRoom } = filesStore;
    const { createTag, fetchTags } = tagsStore;

    const { id } = selectedFolderStore;
    const { updateCurrentFolder } = filesActionsStore;

    return {
      createRoom,
      createTag,
      fetchTags,
      addTagsToRoom,
      currrentFolderId: id,
      updateCurrentFolder,
    };
  }
)(observer(CreateRoomEvent));
