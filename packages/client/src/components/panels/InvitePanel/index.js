import React, { useEffect, useState } from "react";
import debounce from "lodash.debounce";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Base from "@docspace/components/themes/base";

import {
  StyledBlock,
  StyledHeading,
  StyledInvitePanel,
  StyledSubHeader,
  StyledInviteInput,
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
}) => {
  const [items, setItems] = useState([]);
  const [selectedRoom, setSelectedRoom] = useState(null);

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

  const onClose = () => setInvitePanelOptions({ visible: false });

  const onAddUser = (user) => {
    setItems([...items, user]);
  };

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  const onSelectItemAccess = (selectedItem) => {
    if (selectedItem.key === "delete") {
      const newItems = items.filter((item) => item.id !== selectedItem.id);
      return setItems(newItems);
    }
  };

  return (
    <StyledInvitePanel>
      <Backdrop onClick={onClose} visible isAside={true} zIndex={210} />
      <Aside className="invite_panel" visible onClose={onClose}>
        <StyledBlock>
          <StyledHeading>Invite users to the room</StyledHeading>
        </StyledBlock>

        <StyledBlock noPadding>
          <StyledSubHeader>External link</StyledSubHeader>
        </StyledBlock>

        <StyledSubHeader>Individual invitation</StyledSubHeader>
        <InviteInput getUsersByQuery={getUsersByQuery} onAddUser={onAddUser} />
        {!!items.length && (
          <Items items={items} onSelectItemAccess={onSelectItemAccess} />
        )}
      </Aside>
    </StyledInvitePanel>
  );
};

InvitePanel.defaultProps = { theme: Base };

export default inject(({ auth, peopleStore, filesStore }) => {
  const {
    invitePanelOptions,
    setInvitePanelOptions,
    theme,
  } = auth.settingsStore;

  const { getUsersByQuery } = peopleStore.usersStore;

  const { getFolderInfo, folders } = filesStore;

  return {
    invitePanelOptions,
    setInvitePanelOptions,
    visible: invitePanelOptions.visible,
    theme,
    getUsersByQuery,
    getFolderInfo,
    folders,
  };
})(
  withTranslation(["HotkeysPanel", "Article", "Common"])(observer(InvitePanel))
);
