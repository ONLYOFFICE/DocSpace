import React, { useEffect, useState } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import CatalogItem from "@appserver/components/catalog-item";
import { FolderType, ShareAccessRights } from "@appserver/common/constants";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@appserver/components/drag-and-drop";
import withLoader from "../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";
import Loader from "@appserver/components/loader";
import { isMobile } from "react-device-detect";

const StyledDragAndDrop = styled(DragAndDrop)`
  display: contents;
`;

const Item = ({
  t,
  item,
  dragging,
  getFolderIcon,
  isActive,
  getEndOfBlock,
  showText,
  onClick,
  onMoveTo,
  onBadgeClick,
  showDragItems,
  startUpload,
  uploadEmptyFolders,
  setDragging,
  showBadge,
  labelBadge,
  iconBadge,
}) => {
  const [isDragActive, setIsDragActive] = React.useState(false);

  const isDragging = dragging ? showDragItems(item) : false;

  let value = "";
  if (isDragging) value = `${item.id} dragging`;

  const onDropZoneUpload = React.useCallback(
    (files, uploadToFolder) => {
      dragging && setDragging(false);
      const emptyFolders = files.filter((f) => f.isEmptyDirectory);

      if (emptyFolders.length > 0) {
        uploadEmptyFolders(emptyFolders, uploadToFolder).then(() => {
          const onlyFiles = files.filter((f) => !f.isEmptyDirectory);
          if (onlyFiles.length > 0) startUpload(onlyFiles, uploadToFolder, t);
        });
      } else {
        startUpload(files, uploadToFolder, t);
      }
    },
    [t, dragging, setDragging, startUpload, uploadEmptyFolders]
  );

  const onDrop = React.useCallback(
    (items) => {
      if (!isDragging) return dragging && setDragging(false);

      const { fileExst, id } = item;

      if (!fileExst) {
        onDropZoneUpload(items, id);
      } else {
        onDropZoneUpload(items);
      }
    },
    [item, startUpload, dragging, setDragging]
  );

  const onDragOver = React.useCallback(
    (dragActive) => {
      if (dragActive !== isDragActive) {
        setIsDragActive(dragActive);
      }
    },
    [isDragActive]
  );

  const onDragLeave = React.useCallback(() => {
    setIsDragActive(false);
  }, []);

  return (
    <StyledDragAndDrop
      key={item.id}
      data-title={item.title}
      value={value}
      onDrop={onDrop}
      dragging={dragging && isDragging}
      onDragOver={onDragOver}
      onDragLeave={onDragLeave}
    >
      <CatalogItem
        key={item.id}
        id={item.id}
        className={`tree-drag ${item.folderClassName}`}
        icon={getFolderIcon(item)}
        showText={showText}
        text={item.title}
        isActive={isActive(item)}
        onClick={onClick}
        onDrop={onMoveTo}
        isEndOfBlock={getEndOfBlock(item)}
        isDragging={isDragging}
        isDragActive={isDragActive && isDragging}
        value={value}
        showBadge={showBadge}
        labelBadge={labelBadge}
        onClickBadge={onBadgeClick}
        iconBadge={iconBadge}
      />
    </StyledDragAndDrop>
  );
};

let dataMainTree = [];
const Items = ({
  t,
  data,
  showText,
  pathParts,
  selectedTreeNode,
  onClick,
  onBadgeClick,

  dragging,
  setDragging,
  startUpload,
  uploadEmptyFolders,

  isAdmin,
  myId,
  commonId,
  currentId,
  draggableItems,

  moveDragItems,

  setEmptyTrashDialogVisible,
  trashIsEmpty,

  onHide,
}) => {
  useEffect(() => {
    data.forEach((elem) => {
      const elemId = elem.id;
      dataMainTree.push(elemId.toString());
    });
  }, [data]);

  const isActive = React.useCallback(
    (item) => {
      if (selectedTreeNode.length > 0) {
        const isMainFolder = dataMainTree.indexOf(selectedTreeNode[0]) !== -1;

        if (pathParts && pathParts.includes(item.id) && !isMainFolder)
          return true;

        if (selectedTreeNode[0] === "@my" && item.key === "0-0") return true;
        return `${item.id}` === selectedTreeNode[0];
      }
    },
    [selectedTreeNode, pathParts]
  );
  const getEndOfBlock = React.useCallback((item) => {
    switch (item.key) {
      case "0-3":
      case "0-5":
      case "0-6":
        return true;
      default:
        return false;
    }
  }, []);

  const getFolderIcon = React.useCallback((item) => {
    let iconUrl = "images/catalog.folder.react.svg";

    switch (item.rootFolderType) {
      case FolderType.USER:
        iconUrl = "/static/images/catalog.user.react.svg";
        break;
      case FolderType.SHARE:
        iconUrl = "/static/images/catalog.shared.react.svg";
        break;
      case FolderType.COMMON:
        iconUrl = "/static/images/catalog.portfolio.react.svg";
        break;
      case FolderType.Favorites:
        iconUrl = "/static/images/catalog.favorites.react.svg";
        break;
      case FolderType.Recent:
        iconUrl = "/static/images/catalog.recent.react.svg";
        break;
      case FolderType.Privacy:
        iconUrl = "/static/images/catalog.private.react.svg";
        break;
      case FolderType.TRASH:
        iconUrl = "/static/images/catalog.trash.react.svg";
        break;
      default:
        break;
    }

    switch (item.providerKey) {
      case "GoogleDrive":
        iconUrl = "/static/images/cloud.services.google.drive.react.svg";
        break;
      case "Box":
        iconUrl = "/static/images/cloud.services.box.react.svg";
        break;
      case "DropboxV2":
        iconUrl = "/static/images/cloud.services.dropbox.react.svg";
        break;
      case "OneDrive":
        iconUrl = "/static/images/cloud.services.onedrive.react.svg";
        break;
      case "SharePoint":
        iconUrl = "/static/images/cloud.services.onedrive.react.svg";
        break;
      case "kDrive":
        iconUrl = "/static/images/catalog.folder.react.svg";
        break;
      case "Yandex":
        iconUrl = "/static/images/catalog.folder.react.svg";
        break;
      case "NextCloud":
        iconUrl = "/static/images/cloud.services.nextcloud.react.svg";
        break;
      case "OwnCloud":
        iconUrl = "/static/images/catalog.folder.react.svg";
        break;
      case "WebDav":
        iconUrl = "/static/images/catalog.folder.react.svg";
        break;
      default:
        break;
    }

    return iconUrl;
  }, []);

  const showDragItems = React.useCallback(
    (item) => {
      if (item.id === currentId) {
        return false;
      }

      if (!draggableItems || draggableItems.find((x) => x.id === item.id))
        return false;

      if (
        item.rootFolderType === FolderType.SHARE &&
        item.access === ShareAccessRights.FullAccess
      ) {
        return true;
      }

      if (isAdmin) {
        if (
          (item.pathParts &&
            (item.pathParts[0] === myId || item.pathParts[0] === commonId)) ||
          item.rootFolderType === FolderType.USER ||
          item.rootFolderType === FolderType.COMMON
        ) {
          return true;
        }
      } else {
        if (
          (item.pathParts && item.pathParts[0] === myId) ||
          item.rootFolderType === FolderType.USER
        ) {
          return true;
        }
      }

      return false;
    },
    [currentId, draggableItems, isAdmin]
  );

  const onMoveTo = React.useCallback(
    (destFolderId, title) => {
      moveDragItems(destFolderId, title, null, {
        copy: t("Translations:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      });
    },
    [moveDragItems, t]
  );

  const onEmptyTrashAction = () => {
    isMobile && onHide();
    setEmptyTrashDialogVisible(true);
  };

  const getItem = React.useCallback(
    (data) => {
      const items = data.map((item, index) => {
        const isTrash = item.rootFolderType === FolderType.TRASH;
        const showBadge = item.newItems
          ? item.newItems > 0 && true
          : isTrash && !trashIsEmpty;
        const labelBadge = showBadge ? item.newItems : null;
        const iconBadge = isTrash ? "images/clear.trash.react.svg" : null;

        return (
          <Item
            key={`${item.id}_${index}`}
            t={t}
            setDragging={setDragging}
            startUpload={startUpload}
            uploadEmptyFolders={uploadEmptyFolders}
            item={item}
            dragging={dragging}
            getFolderIcon={getFolderIcon}
            isActive={isActive}
            getEndOfBlock={getEndOfBlock}
            showText={showText}
            onClick={onClick}
            onMoveTo={onMoveTo}
            onBadgeClick={isTrash ? onEmptyTrashAction : onBadgeClick}
            showDragItems={showDragItems}
            showBadge={showBadge}
            labelBadge={labelBadge}
            iconBadge={iconBadge}
          />
        );
      });
      return items;
    },
    [
      t,
      dragging,
      getFolderIcon,
      isActive,
      onClick,
      onMoveTo,
      getEndOfBlock,
      onBadgeClick,
      showDragItems,
      showText,
      setDragging,
      startUpload,
      uploadEmptyFolders,
      trashIsEmpty,
    ]
  );

  return <>{getItem(data)}</>;
};

Items.propTypes = {
  data: PropTypes.array,
  showText: PropTypes.bool,
  selectedTreeNode: PropTypes.array,
  onClick: PropTypes.func,
  onClickBadge: PropTypes.func,
  onHide: PropTypes.func,
};

export default inject(
  ({
    auth,
    treeFoldersStore,
    selectedFolderStore,
    filesStore,
    filesActionsStore,
    uploadDataStore,
    dialogsStore,
  }) => {
    const {
      selection,
      dragging,
      setDragging,
      setStartDrag,
      trashIsEmpty,
    } = filesStore;

    const { startUpload } = uploadDataStore;

    const {
      selectedTreeNode,
      treeFolders,
      myFolderId,
      commonFolderId,
      isPrivacyFolder,
    } = treeFoldersStore;

    const { id } = selectedFolderStore;
    const { moveDragItems, uploadEmptyFolders } = filesActionsStore;
    const { setEmptyTrashDialogVisible } = dialogsStore;

    return {
      isAdmin: auth.isAdmin,
      myId: myFolderId,
      commonId: commonFolderId,
      isPrivacy: isPrivacyFolder,
      currentId: id,
      showText: auth.settingsStore.showText,
      pathParts: selectedFolderStore.pathParts,
      data: treeFolders,
      selectedTreeNode,
      draggableItems: dragging ? selection : null,
      dragging,
      setDragging,
      setStartDrag,
      moveDragItems,
      startUpload,
      uploadEmptyFolders,
      setEmptyTrashDialogVisible,
      trashIsEmpty,
    };
  }
)(withTranslation(["Home", "Common", "Translations"])(observer(Items)));
