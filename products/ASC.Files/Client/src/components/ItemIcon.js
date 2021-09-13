import React from "react";
import { EncryptedFileIcon } from "./Icons";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

const StyledIcon = styled.img`
  width: 24px;
  height: 24px;
  margin-top: 4px;
`;

const ItemIcon = ({
  id,
  icon,
  fileExst,
  isPrivacy,
  viewAs,
  actionType,
  actionExtension,
  actionId,
}) => {
  const isEdit =
    (actionType !== null && actionId === id && fileExst === actionExtension) ||
    id <= 0;

  return (
    <>
      <StyledIcon
        className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
        src={icon}
      />
      {isPrivacy && fileExst && (
        <EncryptedFileIcon isEdit={isEdit && viewAs !== "tile"} />
      )}
    </>
  );
};

export default inject(({ filesStore, treeFoldersStore }) => {
  const { type, extension, id } = filesStore.fileActionStore;

  return {
    viewAs: filesStore.viewAs,
    isPrivacy: treeFoldersStore.isPrivacyFolder,
    actionType: type,
    actionExtension: extension,
    actionId: id,
  };
})(observer(ItemIcon));
