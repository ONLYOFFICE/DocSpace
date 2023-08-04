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

  .sharing_panel-arrow svg {
    ${({ theme }) =>
      theme.interfaceDirection === "rtl" && `transform: scaleX(-1);`}
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

  fetchedTags,
  isLoading,
  setIsLoading,

  deleteThirdParty,
  fetchThirdPartyProviders,
  enableThirdParty,
}) => {
  const [isScrollLocked, setIsScrollLocked] = useState(false);
  const [isOauthWindowOpen, setIsOauthWindowOpen] = useState(false);

  const isMountRef = React.useRef(true);

  React.useEffect(() => {
    return () => {
      isMountRef.current = false;
    };
  });

  const startRoomParams = {
    type: undefined,
    title: "",
    tags: [],
    isPrivate: false,
    storageLocation: {
      isThirdparty: false,
      provider: null,
      thirdpartyAccount: null,
      storageFolderId: "",
      isSaveThirdpartyAccount: false,
    },
    icon: {
      uploadedFile: null,
      tmpFile: "",
      x: 0.5,
      y: 0.5,
      zoom: 1,
    },
  };

  const [roomParams, setRoomParams] = useState({ ...startRoomParams });
  const [isValidTitle, setIsValidTitle] = useState(true);

  const setRoomTags = (newTags) =>
    setRoomParams({ ...roomParams, tags: newTags });

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags, fetchedTags);

  const setRoomType = (newRoomType) => {
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));
  };

  const isRoomTitleChanged = roomParams.title.trim() !== "" ? false : true;

  const onKeyUpHandler = (e) => {
    if (e.keyCode === 13) onCreateRoom();
  };

  const onCreateRoom = async () => {
    if (!roomParams.title.trim()) {
      setIsValidTitle(false);
      return;
    }

    await onCreate({ ...roomParams });
    if (isMountRef.current) {
      setRoomParams(startRoomParams);
    }
  };

  const goBack = () => {
    if (isLoading) return;
    setRoomParams({ ...startRoomParams });
  };

  const onCloseAndDisconnectThirdparty = async () => {
    if (isLoading) return;

    if (!!roomParams.storageLocation.thirdpartyAccount) {
      setIsLoading(true);
      await deleteThirdParty(
        roomParams.storageLocation.thirdpartyAccount.providerId
      ).finally(() => setIsLoading(false));

      await fetchThirdPartyProviders();
    }
    onClose();
  };

  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onCloseAndDisconnectThirdparty}
      isScrollLocked={isScrollLocked}
      withFooterBorder
      isOauthWindowOpen={isOauthWindowOpen}
    >
      <ModalDialog.Header>
        <DialogHeader
          isChooseRoomType={!roomParams.type}
          onArrowClick={goBack}
        />
      </ModalDialog.Header>

      <ModalDialog.Body>
        {!roomParams.type ? (
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
            isDisabled={isLoading}
            isValidTitle={isValidTitle}
            setIsValidTitle={setIsValidTitle}
            enableThirdParty={enableThirdParty}
            onKeyUp={onKeyUpHandler}
          />
        )}
      </ModalDialog.Body>

      {!!roomParams.type && (
        <ModalDialog.Footer>
          <Button
            id="shared_create-room-modal_submit"
            tabIndex={5}
            label={t("Common:Create")}
            size="normal"
            primary
            scale
            onClick={onCreateRoom}
            isDisabled={isRoomTitleChanged}
            isLoading={isLoading}
          />
          <Button
            id="shared_create-room-modal_cancel"
            tabIndex={5}
            label={t("Common:CancelButton")}
            size="normal"
            scale
            isDisabled={isLoading}
            onClick={onCloseAndDisconnectThirdparty}
          />
        </ModalDialog.Footer>
      )}
    </StyledModalDialog>
  );
};

export default CreateRoomDialog;
