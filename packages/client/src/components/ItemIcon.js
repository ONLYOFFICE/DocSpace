import SecuritySvgUrl from "ASSETS_DIR/images/security.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledIcon = styled.img`
  /* width: 24px;
  height: 24px;
  margin-top: 4px; */

  ${(props) =>
    props.isRoom &&
    css`
      border-radius: 6px;
      outline: 1px solid ${(props) => props.theme.itemIcon.borderColor};
    `}

  ${(props) =>
    props.isHidden &&
    css`
      display: none;
      border-radius: none;
      outline: none;
    `}
`;

StyledIcon.defaultProps = { theme: Base };

const EncryptedFileIcon = styled.div`
  background: url(${SecuritySvgUrl}) no-repeat 0 0 / 16px 16px transparent;
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
      {isRoom && (
        <StyledIcon
          className={`react-svg-icon`}
          isHidden={!showDefaultIcon}
          isRoom={isRoom}
          src={defaultRoomIcon}
        />
      )}
      <StyledIcon
        className={`react-svg-icon`}
        isHidden={showDefaultIcon}
        isRoom={isRoom}
        src={icon}
        onLoad={onLoadRoomIcon}
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
