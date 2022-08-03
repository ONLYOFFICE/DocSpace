import React, { useState } from "react";
import styled from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";

import TagHandler from "./handlers/tagHandler";

import SetRoomParams from "./sub-components/SetRoomParams";
import RoomTypeList from "./sub-components/RoomTypeList";
import { roomTypes } from "./data";
import IconButton from "@docspace/components/icon-button";

const StyledModalDialog = styled(ModalDialog)`
  .header-with-button {
    display: flex;
    align-items: center;
    flex-direction: row;
    gap: 12px;
  }
`;

const CreateRoomDialog = ({
  t,
  visible,
  onClose,
  onCreate,

  fetchedTags,
  isLoading,
  folderFormValidation,
}) => {
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

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags, fetchedTags);

  const setRoomType = (newRoomType) => {
    const [roomByType] = roomTypes.filter((room) => room.type === newRoomType);
    tagHandler.refreshDefaultTag(t(roomByType.defaultTag));
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));
  };

  const onCreateRoom = () => {
    onCreate(roomParams);
  };

  const isChooseRoomType = roomParams.type === undefined;
  const goBack = () => {
    setRoomParams({
      title: "",
      type: undefined,
      tags: [],
      isPrivate: false,
      storageLocation: undefined,
      rememberStorageLocation: false,
      thirdpartyFolderName: "",
      icon: "",
    });
  };

  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      isScrollLocked={isScrollLocked}
      withFooterBorder
    >
      <ModalDialog.Header>
        {isChooseRoomType ? (
          t("ChooseRoomType")
        ) : (
          <div className="header-with-button">
            <IconButton
              size="15px"
              iconName="/static/images/arrow.path.react.svg"
              className="sharing_panel-arrow"
              onClick={goBack}
            />
            <div>{t("CreateRoom")}</div>
          </div>
        )}
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
      )}
    </StyledModalDialog>
  );
};

export default CreateRoomDialog;
