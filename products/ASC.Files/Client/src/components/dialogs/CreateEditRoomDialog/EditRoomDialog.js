import React, { useState } from "react";

import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";

import TagHandler from "./handlers/tagHandler";
import { roomTypes } from "./data";

import SetRoomParams from "./sub-components/SetRoomParams";

const EditRoomDialog = ({
  t,
  visible,
  onClose,
  onSave,
  isLoading,
  fetchedRoomParams,
  fetchedTags,
  folderFormValidation,
}) => {
  const [isScrollLocked, setIsScrollLocked] = useState(false);

  const [roomParams, setRoomParams] = useState({
    ...fetchedRoomParams,
    // cant fetch
    isPrivate: false,
    storageLocation: undefined,
    rememberStorageLocation: false,
    thirdpartyFolderName: "",
  });

  const setRoomTags = (newTags) =>
    setRoomParams({ ...roomParams, tags: newTags });

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags, fetchedTags);

  const setRoomType = (newRoomType) => {
    const [roomByType] = roomTypes.filter((room) => room.type === newRoomType);
    tagHandler.refreshDefaultTag(t(roomByType.title));
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));
  };

  const onEditRoom = () => {
    onSave(roomParams);
  };

  return (
    <ModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      isScrollLocked={isScrollLocked}
      withFooterBorder
    >
      <ModalDialog.Header>{t("RoomEditing")}</ModalDialog.Header>

      <ModalDialog.Body>
        <SetRoomParams
          t={t}
          tagHandler={tagHandler}
          roomParams={roomParams}
          setRoomParams={setRoomParams}
          setRoomType={setRoomType}
          setIsScrollLocked={setIsScrollLocked}
          isEdit
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          tabIndex={5}
          label={t("Common:SaveButton")}
          size="normal"
          primary
          scale
          onClick={onEditRoom}
          isLoading={isLoading}
        />
        <Button
          tabIndex={5}
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default EditRoomDialog;
