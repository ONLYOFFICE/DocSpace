import React, { useState } from "react";
import styled, { css } from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";

import TagHandler from "./handlers/TagHandler";

import SetRoomParams from "./sub-components/SetRoomParams";
import RoomTypeList from "./sub-components/RoomTypeList";
import DialogHeader from "./sub-components/DialogHeader";

const StyledModalDialog = styled(ModalDialog)`
  .header-with-button {
    display: flex;
    align-items: center;
    flex-direction: row;
    gap: 12px;
  }

  ${(props) =>
    props.isOauthWindowOpen &&
    css`
      #modal-dialog {
        display: none;
      }
    `}
`;

const CreateRoomDialog = ({
  t,
  visible,
  onClose,
  onCreate,

  connectItems,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdpartyResponse,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,

  fetchedTags,
  isLoading,
  folderFormValidation,
}) => {
  const [isScrollLocked, setIsScrollLocked] = useState(false);
  const [isOauthWindowOpen, setIsOauthWindowOpen] = useState(false);

  const startRoomParams = {
    title: "",
    type: undefined,
    tags: [],
    isPrivate: false,
    isThirdparty: false,
    storageLocation: {
      isConnected: false,
      provider: null,
      thirdpartyFolderId: "",
      storageFolderPath: "",
    },
    rememberThirdpartyStorage: false,
    icon: {
      uploadedFile: null,
      tmpFile: "",
      x: 0.5,
      y: 0.5,
      zoom: 1,
    },
  };

  const [roomParams, setRoomParams] = useState({ ...startRoomParams });

  const setRoomTags = (newTags) =>
    setRoomParams({ ...roomParams, tags: newTags });

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags, fetchedTags);

  const setRoomType = (newRoomType) => {
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));
  };

  const onCreateRoom = () => onCreate(roomParams);

  const isChooseRoomType = roomParams.type === undefined;
  const goBack = () => {
    setRoomParams({ ...startRoomParams });
  };

  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      isScrollLocked={isScrollLocked}
      withFooterBorder
      isOauthWindowOpen={isOauthWindowOpen}
    >
      <ModalDialog.Header>
        <DialogHeader
          isChooseRoomType={isChooseRoomType}
          onArrowClick={goBack}
        />
      </ModalDialog.Header>

      <ModalDialog.Body>
        {isChooseRoomType ? (
          <RoomTypeList t={t} setRoomType={setRoomType} />
        ) : (
          <SetRoomParams
            t={t}
            setIsOauthWindowOpen={setIsOauthWindowOpen}
            tagHandler={tagHandler}
            roomParams={roomParams}
            setRoomParams={setRoomParams}
            setRoomType={setRoomType}
            setIsScrollLocked={setIsScrollLocked}
            connectItems={connectItems}
            setConnectDialogVisible={setConnectDialogVisible}
            setRoomCreation={setRoomCreation}
            saveThirdpartyResponse={saveThirdpartyResponse}
            openConnectWindow={openConnectWindow}
            setConnectItem={setConnectItem}
            getOAuthToken={getOAuthToken}
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
