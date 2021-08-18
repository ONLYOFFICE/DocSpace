import React from "react";
import { ReactSVG } from "react-svg";
import { EncryptedFileIcon } from "./Icons";
import { inject, observer } from "mobx-react";

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

  const svgLoader = () => <div style={{ width: "24px" }}></div>;

  return (
    <>
      <ReactSVG
        className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
        src={icon}
        loading={svgLoader}
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
