import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobileOnly } from "react-device-detect";

import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import Portal from "@docspace/components/portal";

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
  getPortalInviteLinks,
  userLink,
  guestLink,
  adminLink,
  defaultAccess,
  inviteUsers,
  setInfoPanelIsMobileHidden,
  reloadSelectionParentRoom,
  setUpdateRoomMembers,
  roomsView,
}) => {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [hasErrors, setHasErrors] = useState(false);
  const [shareLinks, setShareLinks] = useState([]);
  const [roomUsers, setRoomUsers] = useState([]);

  const selectRoom = () => {
    const room = folders.find((folder) => folder.id === roomId);

    if (room) {
      setSelectedRoom(room);
    } else {
      getFolderInfo(roomId).then((info) => {
        setSelectedRoom(info);
      });
    }
  };

  const getInfo = () => {
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
            access: defaultAccess,
          });
        }
      });

      setShareLinks(links);
      setRoomUsers(users);
    });
  };

  useEffect(() => {
    if (roomId === -1) {
      if (!userLink || !guestLink || !adminLink) getPortalInviteLinks();

      setShareLinks([
        {
          id: "user",
          title: "User",
          shareLink: userLink,
          access: 1,
        },
        {
          id: "guest",
          title: "Guest",
          shareLink: guestLink,
          access: 2,
        },
        {
          id: "admin",
          title: "Admin",
          shareLink: adminLink,
          access: 3,
        },
      ]);

      return;
    }

    selectRoom();
    getInfo();
  }, [roomId, userLink, guestLink, adminLink]);

  useEffect(() => {
    const hasErrors = inviteItems.some((item) => !!item.errors?.length);

    setHasErrors(hasErrors);
  }, [inviteItems]);

  const onClose = () => {
    setInfoPanelIsMobileHidden(false);
    setInvitePanelOptions({
      visible: false,
      hideSelector: false,
      defaultAccess: 1,
    });
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
      let newItem = {};

      roomId === -1
        ? (newItem.type = item.access)
        : (newItem.access = item.access);

      item.avatar ? (newItem.id = item.id) : (newItem.email = item.email);

      return newItem;
    });

    const data = {
      invitations,
    };

    if (roomId !== -1) {
      data.notify = true;
      data.message = "Invitation message";
    }

    try {
      roomId === -1
        ? await inviteUsers(data)
        : await setRoomSecurity(roomId, data);

      if (roomsView === "info_members") setUpdateRoomMembers(true);
      onClose();
      toastr.success(t("Common:UsersInvited"));
      reloadSelectionParentRoom();
    } catch (err) {
      toastr.error(err);
    }
  };

  const roomType = selectedRoom ? selectedRoom.roomType : -1;

  const invitePanelComponent = (
    <StyledInvitePanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={isMobileOnly ? 10 : 210}
      />
      <Aside
        className="invite_panel"
        visible={visible}
        onClose={onClose}
        withoutBodyScroll
        zIndex={310}
      >
        <StyledBlock>
          <StyledHeading>
            {roomId === -1 ? t("Common:InviteUsers") : t("InviteUsersToRoom")}
          </StyledHeading>
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

  const renderPortalInvitePanel = () => {
    const rootElement = document.getElementById("root");

    return (
      <Portal
        element={invitePanelComponent}
        appendTo={rootElement}
        visible={visible}
      />
    );
  };

  return isMobileOnly ? renderPortalInvitePanel() : invitePanelComponent;
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;

  const { getUsersByQuery, inviteUsers } = peopleStore.usersStore;
  const {
    setIsMobileHidden: setInfoPanelIsMobileHidden,
    reloadSelectionParentRoom,
    setUpdateRoomMembers,
    roomsView,
    filesView,
  } = auth.infoPanelStore;

  const {
    getPortalInviteLinks,
    userLink,
    guestLink,
    adminLink,
  } = peopleStore.inviteLinksStore;

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
    defaultAccess: invitePanelOptions.defaultAccess,
    getFolderInfo,
    getPortalInviteLinks,
    userLink,
    guestLink,
    adminLink,
    inviteUsers,
    setInfoPanelIsMobileHidden,
    reloadSelectionParentRoom,
    setUpdateRoomMembers,
    roomsView,
  };
})(
  withTranslation([
    "InviteDialog",
    "SharingPanel",
    "Translations",
    "Common",
    "InfoPanel",
  ])(observer(InvitePanel))
);
