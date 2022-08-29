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
  invitePanelVisible,
  setInvitePanelVisible,
  t,
  theme,
  tReady,
  getUsersList,
}) => {
  const testItems = [
    {
      email: "test1@gmail.com",
      id: "1",
      displayName: "Administrator Test1",
      avatarSmall:
        "/storage/userPhotos/root/66faa6e4-f133-11ea-b126-00ffeec8b4ef_orig_46-46.jpeg?_=1380485370",
    },
    {
      email: "test2@gmail.com",
      id: "2",
      displayName: "Administrator Test2",
      avatarSmall:
        "/storage/userPhotos/root/66faa6e4-f133-11ea-b126-00ffeec8b4ef_orig_46-46.jpeg?_=1380485370",
    },
    {
      email: "test3@gmail.com",
      id: "3",
      displayName: "Administrator Test3",
    },
    {
      email: "test4@gmail.com",
      id: "4",
      displayName: "Administrator Test4",
    },
  ];

  const [items, setItems] = useState(testItems);

  const onClose = () => setInvitePanelVisible(false);

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
      <Backdrop
        onClick={onClose}
        visible={invitePanelVisible}
        isAside={true}
        zIndex={210}
      />
      <Aside
        className="invite_panel"
        visible={invitePanelVisible}
        onClose={onClose}
      >
        <StyledBlock>
          <StyledHeading>Invite users to the room</StyledHeading>
        </StyledBlock>

        <StyledBlock noPadding>
          <StyledSubHeader>External link</StyledSubHeader>
        </StyledBlock>

        <StyledSubHeader>Individual invitation</StyledSubHeader>
        <InviteInput getUsersList={getUsersList} onAddUser={onAddUser} />
        <Items items={items} onSelectItemAccess={onSelectItemAccess} />
      </Aside>
    </StyledInvitePanel>
  );
};

InvitePanel.defaultProps = { theme: Base };

export default inject(({ auth, peopleStore }) => {
  const {
    invitePanelVisible,
    setInvitePanelVisible,
    theme,
  } = auth.settingsStore;

  const { getUsersList } = peopleStore.usersStore;

  return {
    invitePanelVisible,
    setInvitePanelVisible,
    theme,
    getUsersList,
  };
})(
  withTranslation(["HotkeysPanel", "Article", "Common"])(observer(InvitePanel))
);
