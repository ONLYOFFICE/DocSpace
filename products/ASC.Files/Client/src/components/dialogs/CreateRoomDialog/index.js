import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import ModalDialog from "@appserver/components/modal-dialog";

import SetRoomParams from "./views/CreateRoom/SetRoomParams";
import RoomTypeList from "./views/ChooseRoomType/RoomTypeList";
import Button from "@appserver/components/button";
import { roomTypes } from "./roomTypes";
import TagHandler from "./handlers/tagHandler";

const CreateRoomDialog = ({
  t,
  visible,
  setCreateRoomDialogVisible,
  createRoom,
}) => {
  const onClose = () => setCreateRoomDialogVisible(false);

  const [roomParams, setRoomParams] = useState({
    title: "",
    type: undefined,
    tags: [],
    isPrivate: false,
    storageLocation: undefined,
    icon: "",
  });

  const setRoomTags = (newTags) =>
    setRoomParams({ ...roomParams, tags: newTags });

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags);

  const setRoomType = (newRoomType) => {
    const [roomByType] = roomTypes.filter((room) => room.type === newRoomType);
    tagHandler.refreshDefaultTag(t(roomByType.title));
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));
  };

  const onCreateRoom = () => {
    //createRoom(roomParams);
    console.log(roomParams);
  };

  const isChooseRoomType = roomParams.type === undefined;
  return (
    <ModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>
        {isChooseRoomType ? t("ChooseRoomType") : t("CreateRoom")}
      </ModalDialog.Header>

      <ModalDialog.Body>
        {isChooseRoomType ? (
          <RoomTypeList t={t} setRoomType={setRoomType} />
        ) : (
          <SetRoomParams
            t={t}
            tagHandler={tagHandler}
            roomParams={roomParams}
            setRoomParams={setRoomParams}
            setRoomType={setRoomType}
          />
        )}
      </ModalDialog.Body>

      {!isChooseRoomType && (
        <ModalDialog.Footer>
          <Button
            tabIndex={5}
            label={t("Common:Create")}
            size="normal"
            primary
            scale
            onClick={onCreateRoom}
          />
          <Button
            tabIndex={5}
            label={t("Common:CancelButton")}
            size="normal"
            scale
            onClick={onClose}
          />
        </ModalDialog.Footer>
      )}
    </ModalDialog>
  );
};

export default inject(({ dialogsStore, roomsStore }) => {
  const {
    createRoomDialogVisible: visible,
    setCreateRoomDialogVisible,
  } = dialogsStore;

  console.log(roomsStore);
  //const { createRoom } = roomsStore;

  return {
    visible,
    setCreateRoomDialogVisible,
    createRoom: () => {},
  };
})(withTranslation(["CreateRoomDialog", "Common"])(observer(CreateRoomDialog)));
