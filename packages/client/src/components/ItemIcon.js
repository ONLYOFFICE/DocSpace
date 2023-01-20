import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledIcon = styled.img`
  ${(props) =>
    props.isRoom &&
    css`
      border-radius: 6px;
      outline: 1px solid
        ${(props) =>
          props.default ? "none" : props.theme.itemIcon.borderColor};
    `}
`;

StyledIcon.defaultProps = { theme: Base };

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
      <StyledIcon
        className={`react-svg-icon`}
        isRoom={isRoom}
        src={showDefaultIcon ? defaultRoomIcon : icon}
        onLoad={onLoadRoomIcon}
        default={showDefaultIcon}
      />
      {isPrivacy && fileExst && <EncryptedFileIcon isEdit={false} />}
    </>
  );
};

export default inject(({ treeFoldersStore }) => {
  return {
    isPrivacy: treeFoldersStore.isPrivacyFolder,
  };
})(observer(ItemIcon));
