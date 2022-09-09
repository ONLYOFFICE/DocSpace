import React, { useEffect, useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";

import { EmbeddingPanel } from "../index";

import {
  StyledBlock,
  StyledHeading,
  StyledInvitePanel,
  StyledSubHeader,
  StyledButtons,
} from "./StyledInvitePanel";

import ItemsList from "./sub-components/ItemsList";
import InviteInput from "./sub-components/InviteInput";

const InvitePanel = ({
  invitePanelOptions,
  setInvitePanelOptions,
  visible,
  t,
  getFolderInfo,
  folders,
  setInviteItems,
  inviteItems,
}) => {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [hasErrors, setHasErrors] = useState(false);

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
  }, [invitePanelOptions]);

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

  const onClickSend = (e) => {
    const toSend = inviteItems.map((item) => {
      let newItem = { access: item.access };

      item.avatarSmall ? (newItem.id = item.id) : (newItem.email = item.email);

      return newItem;
    });

    console.log("send", toSend);
  };

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

        <StyledBlock noPadding>
          <StyledSubHeader>{t("SharingPanel:ExternalLink")}</StyledSubHeader>
        </StyledBlock>

        <InviteInput t={t} onClose={onClose} />

        {!!inviteItems.length && (
          <>
            <ItemsList t={t} />
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
