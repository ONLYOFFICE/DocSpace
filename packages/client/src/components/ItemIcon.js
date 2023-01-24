import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledIcon = styled.img`
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
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: 12px;
`;

const ItemIcon = ({ icon, fileExst, isPrivacy, isRoom, defaultRoomIcon }) => {
  const [showDefaultIcon, setShowDefaultIcon] = React.useState(isRoom);

  const onLoadRoomIcon = () => {
    if (!isRoom || defaultRoomIcon === icon) return;

    setShowDefaultIcon(false);
  };

  return (
    <>
      <IconWrapper isRoom={isRoom} default={showDefaultIcon}>
        <StyledIcon
          className={`react-svg-icon`}
          isRoom={isRoom}
          src={showDefaultIcon ? defaultRoomIcon : icon}
          onLoad={onLoadRoomIcon}
        />
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
