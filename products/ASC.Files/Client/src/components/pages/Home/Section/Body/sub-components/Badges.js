import React from "react";
import { inject, observer } from "mobx-react";
import { ShareAccessRights } from "@appserver/common/constants";
import Badge from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
import {
  StyledFavoriteIcon,
  StyledFileActionsConvertEditDocIcon,
  StyledFileActionsLockedIcon,
} from "./Icons";

const Badges = (props) => {
  const {
    newItems,
    viewAs,
    item,
    canWebEdit,
    isTrashFolder,
    canConvert,
    accessToEdit,

    onFilesClick,
    onClickLock,
    onClickFavorite,
    onShowVersionHistory,
    onBadgeClick,
  } = props;
  const { id, locked, fileStatus, versionGroup, title } = item;
  const { fileExst } = item;

  const showNew = !!newItems;
  return fileExst ? (
    <div className="badges additional-badges">
      {/* TODO: Uncomment after fix conversation {canConvert && !isTrashFolder && (
                  <IconButton
                    onClick={this.setConvertDialogVisible}
                    iconName="FileActionsConvertIcon"
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#A3A9AE"
                    hoverColor="#3B72A7"
                  />
      )} */}
      {canWebEdit && !isTrashFolder && accessToEdit && (
        <IconButton
          onClick={onFilesClick}
          iconName="/static/images/access.edit.react.svg"
          className="badge icons-group"
          size="small"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#3B72A7"
        />
      )}
      {locked && (
        <StyledFileActionsLockedIcon
          className="badge lock-file icons-group"
          size="small"
          data-id={id}
          data-locked={true}
          onClick={onClickLock}
        />
      )}
      {fileStatus === 32 && !isTrashFolder && (
        <StyledFavoriteIcon
          className="favorite icons-group"
          size="small"
          data-action="remove"
          data-id={id}
          data-title={title}
          onClick={onClickFavorite}
        />
      )}
      {fileStatus === 1 && (
        <StyledFileActionsConvertEditDocIcon className="badge" size="small" />
      )}
      {versionGroup > 1 && (
        <Badge
          className="badge-version icons-group"
          backgroundColor="#A3A9AE"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="10px"
          fontWeight={800}
          label={`Ver.${versionGroup}`}
          maxWidth="50px"
          onClick={onShowVersionHistory}
          padding="0 5px"
          data-id={id}
        />
      )}
      {showNew && (
        <Badge
          className="badge-version icons-group"
          backgroundColor="#ED7309"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="10px"
          fontWeight={800}
          label={`New`}
          maxWidth="50px"
          onClick={onBadgeClick}
          padding="0 5px"
          data-id={id}
        />
      )}
    </div>
  ) : (
    showNew && (
      <Badge
        className="new-items"
        backgroundColor="#ED7309"
        borderRadius="11px"
        color="#FFFFFF"
        fontSize="10px"
        fontWeight={800}
        label={newItems}
        maxWidth="50px"
        onClick={onBadgeClick}
        padding="0 5px"
        data-id={id}
      />
    )
  );
};

export default inject(({ filesStore, treeFoldersStore }, { item }) => {
  const { viewAs } = filesStore;
  const { isRecycleBinFolder: isTrashFolder } = treeFoldersStore;
  const { access } = item;
  const accessToEdit =
    access === ShareAccessRights.FullAccess ||
    access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

  return { viewAs, isTrashFolder, accessToEdit };
})(observer(Badges));
