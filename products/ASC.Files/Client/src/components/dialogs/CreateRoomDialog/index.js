import React, { useState } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import ModalDialog from "@appserver/components/modal-dialog";

import SetRoomParams from "./sub-components/CreateRoom";
import RoomTypeList from "./sub-components/ChooseRoomType";
import Button from "@appserver/components/button";
import { roomTypes } from "./data";
import TagHandler from "./handlers/tagHandler";

const StyledModalDialog = styled(ModalDialog)`
  .modal-scroll {
    .scroll-body {
    }
  }
`;
const CreateRoomDialog = ({
  t,
  visible,
  setCreateRoomDialogVisible,
  createRoom,
}) => {
  const onClose = () => setCreateRoomDialogVisible(false);
  const [isScrollLocked, setIsScrollLocked] = useState(false);

  const [roomParams, setRoomParams] = useState({
    title: "",
    type: undefined,
    tags: [],
    isPrivate: false,
    storageLocation: undefined,
    rememberStorageLocation: false,
    thirdpartyFolderName: "",
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
    console.log(roomParams);
  };

  const isChooseRoomType = roomParams.type === undefined;
  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      isScrollLocked={isScrollLocked}
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
            setIsScrollLocked={setIsScrollLocked}
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
    </StyledModalDialog>
  );
};

export default inject(({ dialogsStore, roomsStore }) => {
  const {
    createRoomDialogVisible: visible,
    setCreateRoomDialogVisible,
  } = dialogsStore;

  //const { createRoom } = roomsStore;

  return {
    visible,
    setCreateRoomDialogVisible,
    createRoom: () => {},
  };
})(withTranslation(["CreateRoomDialog", "Common"])(observer(CreateRoomDialog)));
