import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import { RoomsType } from "@appserver/common/constants";
import ModalDialog from "@appserver/components/modal-dialog";

import SetRoomParams from "./sub-components/SetRoomParams";
import RoomTypeList from "./sub-components/RoomTypeList";
import Button from "@appserver/components/button";

const CreateRoomDialog = ({
  t,
  visible,
  setCreateRoomDialogVisible,
  createRoom,
}) => {
  const onClose = () => setCreateRoomDialogVisible(false);

  const [roomParams, setRoomParams] = useState({
    title: "New room",
    type: undefined,
    tags: [],
    isPrivate: false,
    storageLocation: {},
    icon: "",
  });

  const chooseRoomType = (roomType) => {
    setRoomParams({ ...roomParams, type: roomType });
  };

  const onCreateRoom = () => {
    createRoom({
      title: "some text",
      roomType: currentRoomtype,
    });
  };

  const rooms = [
    {
      type: RoomsType.FillingFormsRoom,
      title: t("FillingFormsRoomTitle"),
      description: t("FillingFormsRoomDescription"),
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.EditingRoom,
      title: t("CollaborationRoomTitle"),
      description: t("CollaborationRoomDescription"),
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.ReviewRoom,
      title: t("ReviewRoomTitle"),
      description: t("ReviewRoomDescription"),
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.ReadOnlyRoom,
      title: t("ViewOnlyRoomTitle"),
      description: t("ViewOnlyRoomDescription"),
      withSecondaryInfo: true,
    },
    {
      type: RoomsType.CustomRoom,
      title: t("CustomRoomTitle"),
      description: t("CustomRoomDescription"),
      withSecondaryInfo: false,
    },
  ];

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
          <RoomTypeList rooms={rooms} chooseRoomType={chooseRoomType} />
        ) : (
          <SetRoomParams
            roomParams={roomParams}
            rooms={rooms}
            chooseRoomType={chooseRoomType}
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

export default inject(({ roomsStore, dialogsStore }) => {
  const createRoom = { roomsStore };

  const {
    createRoomDialogVisible: visible,
    setCreateRoomDialogVisible,
  } = dialogsStore;

  return {
    visible,
    setCreateRoomDialogVisible,
    createRoom,
  };
})(withTranslation(["CreateRoomDialog", "Common"])(observer(CreateRoomDialog)));
