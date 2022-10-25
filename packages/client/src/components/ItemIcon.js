import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";
import RoomLogo from "@docspace/components/room-logo";

const StyledIcon = styled.img`
  /* width: 24px;
  height: 24px;
  margin-top: 4px; */
`;

const PrivateRoomIcon = styled.div`
  width: 32px;
  height: 32px;
`;

const EncryptedFileIcon = styled.div`
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: 12px;
`;

const ItemIcon = ({
  id,
  icon,
  fileExst,
  viewAs,
  isRoom,
  roomType,
  isPrivacy,
  // actionType,
  // actionExtension,
  // actionId,
}) => {
  // const isEdit =
  //   (actionType !== null && actionId === id && fileExst === actionExtension) ||
  //   id <= 0;

  // return (
  //   <>
  //     <StyledIcon
  //       className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
  //       src={icon}
  //     />
  //     {isPrivacy && fileExst && (
  //       <EncryptedFileIcon isEdit={isEdit && viewAs !== "tile"} />
  //     )}
  //   </>
  // );

  if (isRoom) {
    return (
      <PrivateRoomIcon>
        <RoomLogo type={roomType} isPrivacy={isPrivacy} />
      </PrivateRoomIcon>
    );
  }

  return (
    <>
      <StyledIcon className={`react-svg-icon`} src={icon} />
      {isPrivacy && fileExst && <EncryptedFileIcon isEdit={false} />}
    </>
  );
};

export default inject(({ filesStore, treeFoldersStore }) => {
  // const { type, extension, id } = filesStore.fileActionStore;

  return {
    viewAs: filesStore.viewAs,
    // isPrivacy: treeFoldersStore.isPrivacyFolder,
    // actionType: type,
    // actionExtension: extension,
    // actionId: id,
  };
})(observer(ItemIcon));
