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
  let isMount = true;

  const [imgSrc, setImgSrc] = React.useState(defaultRoomIcon);

  const fetchImage = async () => {
    const res = await fetch(icon);
    const imageBlob = await res.blob();
    const imageObjectURL = URL.createObjectURL(imageBlob);

    isMount && setImgSrc(imageObjectURL);
  };

  React.useEffect(() => {
    if (!isRoom || icon === defaultRoomIcon) return;
    fetchImage();

    return () => {
      isMount = false;
    };
  }, []);

  return (
    <>
      <StyledIcon
        className={`react-svg-icon`}
        isRoom={isRoom}
        src={!isRoom || icon === defaultRoomIcon ? icon : imgSrc}
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
