import React, { useEffect, useState, useMemo, useRef } from "react";
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
import Scrollbar from "@docspace/components/scrollbar";
import { LinkType } from "../../../helpers/constants";
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
  collaboratorLink,
  defaultAccess,
  inviteUsers,
  setInfoPanelIsMobileHidden,
  reloadSelectionParentRoom,
  setUpdateRoomMembers,
  roomsView,
  getUsersList,
  filter,
}) => {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [hasErrors, setHasErrors] = useState(false);
  const [shareLinks, setShareLinks] = useState([]);
  const [roomUsers, setRoomUsers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [externalLinksVisible, setExternalLinksVisible] = useState(false);
  const [scrollAllPanelContent, setScrollAllPanelContent] = useState(false);
  const [activeLink, setActiveLink] = useState({});

  const inputsRef = useRef();

  const onChangeExternalLinksVisible = (visible) => {
    setExternalLinksVisible(visible);
  };

  const onChangeActiveLink = (activeLink) => {
    setActiveLink(activeLink);
  };

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
        const { shareLink, id, title, expirationDate, linkType } = user.sharedTo;

        if (!!shareLink && linkType === LinkType.Invite) {
          links.push({
            id,
            title,
            shareLink,
            expirationDate,
            access: user.access || defaultAccess,
          });
        }
      });

      setShareLinks(links);
      setRoomUsers(users);
    });
  };

  useEffect(() => {
    if (roomId === -1) {
      if (!userLink || !guestLink || !adminLink || !collaboratorLink)
        getPortalInviteLinks();

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
        {
          id: "collaborator",
          title: "Collaborator",
          shareLink: collaboratorLink,
          access: 4,
        },
      ]);

      return;
    }

    selectRoom();
    getInfo();
  }, [roomId, userLink, guestLink, adminLink, collaboratorLink]);

  useEffect(() => {
    const hasErrors = inviteItems.some((item) => !!item.errors?.length);

    setHasErrors(hasErrors);
  }, [inviteItems]);

  useEffect(() => {
    onCheckHeight();
    window.addEventListener("resize", onCheckHeight);
    return () => {
      window.removeEventListener("resize", onCheckHeight);
    };
  }, []);

  const onCheckHeight = () => {
    setScrollAllPanelContent(window.innerHeight < 1024);
  };

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
      setIsLoading(true);
      roomId === -1
        ? await inviteUsers(data)
        : await setRoomSecurity(roomId, data);

      setIsLoading(false);

      if (roomsView === "info_members") {
        setUpdateRoomMembers(true);
      }

      onClose();
      toastr.success(t("Common:UsersInvited"));
      reloadSelectionParentRoom();
    } catch (err) {
      toastr.error(err);
      setIsLoading(false);
    } finally {
      if (roomId === -1) {
        await getUsersList(filter, false);
      }
    }
  };

  const roomType = selectedRoom ? selectedRoom.roomType : -1;
  const hasInvitedUsers = !!inviteItems.length;

  const bodyInvitePanel = useMemo(() => {
    return (
      <>
        <ExternalLinks
          t={t}
          shareLinks={shareLinks}
          getInfo={getInfo}
          roomType={roomType}
          onChangeExternalLinksVisible={onChangeExternalLinksVisible}
          externalLinksVisible={externalLinksVisible}
          onChangeActiveLink={onChangeActiveLink}
          activeLink={activeLink}
        />

        <InviteInput
          t={t}
          onClose={onClose}
          roomUsers={roomUsers}
          roomType={roomType}
          inputsRef={inputsRef}
        />
        {hasInvitedUsers && (
          <ItemsList
            t={t}
            setHasErrors={setHasErrors}
            roomType={roomType}
            externalLinksVisible={externalLinksVisible}
            scrollAllPanelContent={scrollAllPanelContent}
            inputsRef={inputsRef}
          />
        )}
      </>
    );
  }, [
    t,
    shareLinks,
    getInfo,
    roomType,
    onChangeExternalLinksVisible,
    externalLinksVisible,
    onClose,
    roomUsers,
    setHasErrors,
    scrollAllPanelContent,
    hasInvitedUsers,
  ]);

  const invitePanelComponent = (
    <StyledInvitePanel hasInvitedUsers={hasInvitedUsers}>
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
          <StyledHeading>{t("Common:InviteUsers")}</StyledHeading>
        </StyledBlock>

        {scrollAllPanelContent ? (
          <div className="invite-panel-body">
            <Scrollbar stype="mediumBlack">{bodyInvitePanel}</Scrollbar>
          </div>
        ) : (
          bodyInvitePanel
        )}

        {hasInvitedUsers && (
          <StyledButtons>
            <Button
              className="send-invitation"
              scale={true}
              size={"normal"}
              isDisabled={hasErrors}
              primary
              onClick={onClickSend}
              label={t("SendInvitation")}
              isLoading={isLoading}
            />
            <Button
              className="cancel-button"
              scale={true}
              size={"normal"}
              onClick={onClose}
              label={t("Common:CancelButton")}
              isDisabled={isLoading}
            />
          </StyledButtons>
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

  const { getUsersByQuery, inviteUsers, getUsersList } = peopleStore.usersStore;
  const { filter } = peopleStore.filterStore;
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
    collaboratorLink,
  } = peopleStore.inviteLinksStore;

  const {
    inviteItems,
    invitePanelOptions,
    setInviteItems,
    setInvitePanelOptions,
  } = dialogsStore;

  const { getFolderInfo, setRoomSecurity, getRoomSecurityInfo, folders } =
    filesStore;

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
    collaboratorLink,
    inviteUsers,
    setInfoPanelIsMobileHidden,
    reloadSelectionParentRoom,
    setUpdateRoomMembers,
    roomsView,
    getUsersList,
    filter,
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
