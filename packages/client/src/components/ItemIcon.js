import SecuritySvgUrl from "PUBLIC_DIR/images/security.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import roomsIconsColors from "@docspace/components/utils/roomsIconsColors";

const StyledIcon = styled.img`
  ${NoUserSelect}
  ${(props) =>
    props.isRoom &&
    css`
      border-radius: 6px;
      vertical-align: middle;
    `}
`;

const IconTitleRoomWrapper = styled.div`
  ${NoUserSelect}

  display: flex;
  justify-content: center;
  align-items: center;
  height: 32px;
  width: 32px;
  .room-background {
    height: 32px;
    width: 32px;
    border-radius: 6px;
    vertical-align: middle;
    background: ${(props) => props.color};
    position: absolute;
    ${(props) =>
      !props.theme.isBase &&
      css`
        opacity: 0.1;
      `};
  }

  .room-title {
    font-size: 14px;
    font-weight: 700;
    line-height: 16px;

    position: relative;
    ${(props) =>
      !props.theme.isBase &&
      css`
        color: ${(props) => props.color};
      `};
  }
`;

const IconWrapper = styled.div`
  ${(props) =>
    props.isRoom &&
    css`
      position: relative;
      border-radius: 6px;

      &::before {
        content: "";
        position: absolute;
        top: 0px;
        right: 0px;
        bottom: 0px;
        left: 0px;
        border: 1px solid
          ${(props) =>
            props.default ? "none" : props.theme.itemIcon.borderColor};
        border-radius: 5px;
        overflow: hidden;
      }
    `}
`;

IconWrapper.defaultProps = { theme: Base };

const EncryptedFileIcon = styled.div`
  background: url(${SecuritySvgUrl}) no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: 12px;
`;

const ItemIcon = ({
  icon,
  fileExst,
  isPrivacy,
  isRoom,
  defaultRoomIcon,
  title,
  logo,
}) => {
  const [showDefaultIcon, setShowDefaultIcon] = React.useState(isRoom);

  React.useEffect(() => {
    if (!isRoom || defaultRoomIcon === icon) return;
    setShowDefaultIcon(false);
  }, [isRoom, defaultRoomIcon, icon, setShowDefaultIcon]);

  const randomPropertyValue = (object) => {
    const keys = Object.keys(object);
    if (keys.length > 0) {
      const index = Math.floor(keys.length * Math.random());
      return object[keys[index]];
    }
    return null;
  };

  let roomTitle = title;
  if (title) {
    const titleWithoutTooManySpaces = title.replace(/\s+/g, " ").trim();
    const indexSecondCharacterAfterSpace =
      titleWithoutTooManySpaces.indexOf(" ");
    const secondCharacterAfterSpace =
      indexSecondCharacterAfterSpace === -1
        ? ""
        : titleWithoutTooManySpaces[indexSecondCharacterAfterSpace + 1];

    roomTitle = (title[0] + secondCharacterAfterSpace).toUpperCase();
  }

  const isLoadedRoomIcon = !!logo.large;
  const color = randomPropertyValue(roomsIconsColors);

  return (
    <>
      <IconWrapper isRoom={isRoom} default={showDefaultIcon}>
        {isLoadedRoomIcon || !isRoom ? (
          <StyledIcon
            className={`react-svg-icon`}
            isRoom={isRoom}
            src={showDefaultIcon ? defaultRoomIcon : icon}
          />
        ) : (
          <IconTitleRoomWrapper color={color}>
            <div className="room-background" />
            <div className="room-title">{roomTitle}</div>
          </IconTitleRoomWrapper>
        )}
      </IconWrapper>
      {isPrivacy && fileExst && <EncryptedFileIcon isEdit={false} />}
    </>
  );
};

export default inject(({ treeFoldersStore }) => {
  return {
    isPrivacy: treeFoldersStore.isPrivacyFolder,
  };
})(observer(ItemIcon));
