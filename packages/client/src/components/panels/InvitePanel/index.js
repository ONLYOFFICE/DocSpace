import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";

import {
  StyledBlock,
  StyledHeading,
  StyledInvitePanel,
  StyledButtons,
} from "./StyledInvitePanel";

import ItemsList from "./sub-components/ItemsList";
import InviteInput from "./sub-components/InviteInput";
import ExternalLinks from "./sub-components/ExternalLinks";

const InvitePanel = ({
  folders,
  getFolderInfo,
  inviteItems,
  roomId,
  setInviteItems,
  setInvitePanelOptions,
  t,
  visible,
  setRoomSecurity,
  getRoomSecurityInfo,
}) => {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [hasErrors, setHasErrors] = useState(false);
  const [shareLinks, setShareLinks] = useState([]);
  const [roomUsers, setRoomUsers] = useState([]);

  useEffect(() => {
    const room = folders.find((folder) => folder.id === roomId);

    if (room) {
      setSelectedRoom(room);
    } else {
      getFolderInfo(roomId).then((info) => {
        setSelectedRoom(info);
      });
    }

    getRoomSecurityInfo(roomId).then((users) => {
      let links = [];

      users.map((user) => {
        const { shareLink, id, title, expirationDate } = user.sharedTo;

        if (!!shareLink) {
          links.push({
            id,
            title,
            shareLink,
            expirationDate,
          });
        }
      });

      setShareLinks(links);
      setRoomUsers(users);
    });
  }, [roomId]);

  useEffect(() => {
    const hasErrors = inviteItems.some((item) => !!item.errors?.length);

    setHasErrors(hasErrors);
  }, [inviteItems]);

  const onClose = () => {
    setInvitePanelOptions({ visible: false });
    setInviteItems([]);
  };

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  const onClickSend = async (e) => {
    const invitations = inviteItems.map((item) => {
      let newItem = { access: item.access };

      item.avatar ? (newItem.id = item.id) : (newItem.email = item.email);

      return newItem;
    });

    const data = {
      invitations,
      notify: true,
      message: "Invitation message",
    };

    try {
      await setRoomSecurity(roomId, data);
      onClose();
    } catch (err) {
      console.error(err);
    }
  };

  const roomType = selectedRoom ? selectedRoom.roomType : 5;

  return (
    <StyledInvitePanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside
        className="invite_panel"
        visible={visible}
        onClose={onClose}
        withoutBodyScroll
      >
        <StyledBlock>
          <StyledHeading>{t("InviteUsersToRoom")}</StyledHeading>
        </StyledBlock>

        <ExternalLinks t={t} shareLinks={shareLinks} roomType={roomType} />

        <InviteInput
          t={t}
          onClose={onClose}
          roomUsers={roomUsers}
          roomType={roomType}
        />

        {!!inviteItems.length && (
          <>
            <ItemsList t={t} setHasErrors={setHasErrors} roomType={roomType} />
            <StyledButtons>
              <Button
                scale={true}
                size={"normal"}
                isDisabled={hasErrors}
                primary
                onClick={onClickSend}
                label={t("SendInvitation")}
              />
              <Button
                scale={true}
                size={"normal"}
                onClick={onClose}
                label={t("Common:CancelButton")}
              />
            </StyledButtons>
          </>
        )}
      </Aside>
    </StyledInvitePanel>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;

  const { getUsersByQuery } = peopleStore.usersStore;

  const {
    inviteItems,
    invitePanelOptions,
    setInviteItems,
    setInvitePanelOptions,
  } = dialogsStore;

  const {
    getFolderInfo,
    setRoomSecurity,
    getRoomSecurityInfo,
    folders,
  } = filesStore;

  return {
    folders,
    getUsersByQuery,
    getRoomSecurityInfo,
    inviteItems,
    roomId: invitePanelOptions.roomId,
    setInviteItems,
    setInvitePanelOptions,
    setRoomSecurity,
    theme,
    visible: invitePanelOptions.visible,
    getFolderInfo,
  };
})(
  withTranslation(["InviteDialog", "SharingPanel", "Translations", "Common"])(
    observer(InvitePanel)
  )
);
