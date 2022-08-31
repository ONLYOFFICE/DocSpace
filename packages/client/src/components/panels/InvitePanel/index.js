import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";

import { AddUsersPanel, EmbeddingPanel } from "../index";

import {
  StyledBlock,
  StyledHeading,
  StyledInvitePanel,
  StyledSubHeader,
  StyledButtons,
  StyledLink,
} from "./StyledInvitePanel";

import Items from "./items.js";
import InviteInput from "./InviteInput";

const InvitePanel = ({
  invitePanelOptions,
  setInvitePanelOptions,
  visible,
  t,
  theme,
  tReady,
  getUsersByQuery,
  getFolderInfo,
  folders,
  getShareUsers,
  setInviteItems,
  inviteItems,
}) => {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [showUsersPanel, setShowUsersPanel] = useState(false);
  const [hasErrors, setHasErrors] = useState(false);
  const [roomUsers, setRoomUsers] = useState([]);

  let shareUsers = [];

  useEffect(() => {
    const { id } = invitePanelOptions;
    const room = folders.find((folder) => folder.id === id);
    if (room) {
      setSelectedRoom(room);
    } else {
      getFolderInfo(id).then((info) => {
        setSelectedRoom(info);
      });
    }

    getShareUsers([id]).then((accesses) => {
      shareUsers = accesses;
      const users = accesses.map((user) => {
        return {
          id: user.sharedTo.id,
          email: user.sharedTo.email,
          access: user.access,
        };
      });
      setRoomUsers(users);
    });
  }, [invitePanelOptions]);

  const onClose = () => setInvitePanelOptions({ visible: false });

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  const onSelectItemAccess = (selectedItem) => {
    if (selectedItem.key === "delete") {
      const newItems = inviteItems.filter(
        (item) => item.id !== selectedItem.id
      );
      return setInviteItems(newItems);
    }
  };

  const onClickSend = (e) => {
    const toSend = inviteItems.map((item) => {
      let newItem = { access: item.access };

      item.avatar ? (newItem.id = item.id) : (newItem.email = item.email);

      return newItem;
    });

    console.log("send", toSend);
  };

  const openUserPanel = () => {
    setShowUsersPanel(true);
  };

  return (
    <StyledInvitePanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside className="invite_panel" visible={visible} onClose={onClose}>
        <StyledBlock>
          <StyledHeading>{t("InviteUsersToRoom")}</StyledHeading>
        </StyledBlock>

        <StyledBlock noPadding>
          <StyledSubHeader>{t("SharingPanel:ExternalLink")}</StyledSubHeader>
        </StyledBlock>

        <StyledSubHeader>
          {t("IndividualInvitation")}
          <StyledLink
            fontWeight="600"
            type="action"
            isHovered
            onClick={openUserPanel}
          >
            {t("СhooseFromList")}
          </StyledLink>
        </StyledSubHeader>
        <InviteInput t={t} roomUsers={roomUsers} />
        {!!inviteItems.length && (
          <>
            <Items t={t} />
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

        {showUsersPanel && (
          <AddUsersPanel
            onSharingPanelClose={onClose}
            onClose={() => setShowUsersPanel(false)}
            visible={showUsersPanel}
            shareDataItems={shareUsers}
            setShareDataItems={(item) => console.log(item)}
            //groupsCaption={groupsCaption}
            accessOptions={["FullAccess"]}
            isMultiSelect
            //isEncrypted={isEncrypted}
          />
        )}
      </Aside>
    </StyledInvitePanel>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;

  const { getUsersByQuery } = peopleStore.usersStore;

  const {
    invitePanelOptions,
    setInvitePanelOptions,
    setInviteItems,
    inviteItems,
  } = dialogsStore;

  const { getFolderInfo, folders, getShareUsers } = filesStore;

  return {
    invitePanelOptions,
    setInvitePanelOptions,
    visible: invitePanelOptions.visible,
    theme,
    getUsersByQuery,
    getFolderInfo,
    folders,
    getShareUsers,
    setInviteItems,
    inviteItems,
  };
})(
  withTranslation(["InviteDialog", "SharingPanel", "Common"])(
    observer(InvitePanel)
  )
);
