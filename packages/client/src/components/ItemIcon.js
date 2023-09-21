import SecuritySvgUrl from "PUBLIC_DIR/images/security.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import RoomIcon from "./RoomIcon";

const StyledIcon = styled.img`
  ${NoUserSelect}
  ${(props) =>
    props.isRoom &&
    css`
      border-radius: 6px;
      vertical-align: middle;
    `}
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
        border: 1px solid ${(props) => props.theme.itemIcon.borderColor};
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
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: 12px;
        `
      : css`
          margin-left: 12px;
        `}
`;

const ItemIcon = ({
  icon,
  fileExst,
  isPrivacy,
  isRoom,
  title,
  logo,
  color,
  isArchive,
}) => {
  const isLoadedRoomIcon = !!logo?.medium;
  const showDefaultRoomIcon = !isLoadedRoomIcon && isRoom;

  return (
    <>
      <IconWrapper isRoom={isRoom}>
        {showDefaultRoomIcon ? (
          <RoomIcon color={color} title={title} isArchive={isArchive} />
        ) : (
          <StyledIcon
            className={`react-svg-icon`}
            isRoom={isRoom}
            src={isRoom ? logo?.medium : icon}
          />
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
