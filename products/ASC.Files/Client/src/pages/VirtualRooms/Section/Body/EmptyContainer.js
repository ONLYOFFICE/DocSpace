import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";

import { default as Container } from "../../../../components/EmptyContainer/EmptyContainer";

import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";

import { tablet } from "@appserver/components/utils/device";
import { Events } from "../../../../helpers/constants";

const StyledWrapper = styled.div`
  .empty-folder_container {
    grid-row-gap: 8px;
    grid-column-gap: 40px;

    height: 100px;

    align-items: center;

    grid-row-gap: 8px;
    grid-column-gap: 40px;

    height: 100px;

    @media ${tablet} {
      padding: 14px 0;
    }

    ${isMobile &&
    css`
      padding: 14px 0;
    `}
  }
`;

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  className: "empty-folder_link",
  display: "flex",
};

const EmptyContainer = ({ theme, setCreateRoomDialogVisible }) => {
  linkStyles.color = theme.filesEmptyContainer.linkColor;

  const onCreateRoom = () => {
    const event = new Event(Events.ROOM_CREATE);
    window.dispatchEvent(event);
  };

  const buttons = (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        iconName="images/plus.svg"
        isFill
        onClick={onCreateRoom}
      />
      <Link onClick={onCreateRoom} {...linkStyles}>
        Create room
      </Link>
    </div>
  );

  return (
    <StyledWrapper>
      <Container
        headerText={"Welcome to virtual rooms!"}
        descriptionText={"Please create the first room."}
        imageSrc="images/empty_screen_corporate.png"
        buttons={buttons}
      />
    </StyledWrapper>
  );
};

export default inject(({ auth, dialogsStore }) => {
  const { theme } = auth.settingsStore;
  const { setCreateRoomDialogVisible } = dialogsStore;
  return {
    theme,
    setCreateRoomDialogVisible,
  };
})(withTranslation(["Home", "Common"])(observer(EmptyContainer)));
