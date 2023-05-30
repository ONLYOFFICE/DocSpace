import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import ClearTrashReactSvgUrl from "PUBLIC_DIR/images/clear.trash.react.svg?url";
import CatalogUserReactSvgUrl from "PUBLIC_DIR/images/catalog.user.react.svg?url";
import CatalogArchiveReactSvgUrl from "PUBLIC_DIR/images/catalog.archive.react.svg?url";
import CatalogSharedReactSvgUrl from "PUBLIC_DIR/images/catalog.shared.react.svg?url";
import CatalogPortfolioReactSvgUrl from "PUBLIC_DIR/images/catalog.portfolio.react.svg?url";
import CatalogFavoritesReactSvgUrl from "PUBLIC_DIR/images/catalog.favorites.react.svg?url";
import CatalogRecentReactSvgUrl from "PUBLIC_DIR/images/catalog.recent.react.svg?url";
import CatalogPrivateReactSvgUrl from "PUBLIC_DIR/images/catalog.private.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";
import React, { useEffect, useState } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import CatalogItem from "@docspace/components/catalog-item";
import {
  FolderType,
  ShareAccessRights,
  FolderNames,
} from "@docspace/common/constants";
import { withTranslation } from "react-i18next";
import DragAndDrop from "@docspace/components/drag-and-drop";
import { isMobile } from "react-device-detect";
import SettingsItem from "./SettingsItem";
import AccountsItem from "./AccountsItem";

const StyledDragAndDrop = styled(DragAndDrop)`
  display: contents;
`;

const CatalogDivider = styled.div`
  height: 16px;
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
  folderId,
}) => {
  const [isDragActive, setIsDragActive] = useState(false);

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
        folderId={folderId}
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
        badgeTitle={t("RecycleBinAction")}
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
  rootFolderType,
  selectedTreeNode,
  onClick,
  onBadgeClick,

  dragging,
  setDragging,
  startUpload,
  uploadEmptyFolders,
  isVisitor,
  isCollaborator,
  isAdmin,
  myId,
  commonId,
  currentId,
  draggableItems,

  moveDragItems,

  docSpace,

  setEmptyTrashDialogVisible,
  trashIsEmpty,

  onHide,
  firstLoad,
  deleteAction,
  startDrag,
  emptyTrashInProgress,
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

        if (
          rootFolderType === FolderType.Rooms &&
          item.rootFolderType === FolderType.Rooms
        ) {
          return true;
        }

        if (pathParts && pathParts.includes(item.id) && !isMainFolder)
          return true;

        if (
          (selectedTreeNode[0] === "@my" || selectedTreeNode[0] === "@rooms") &&
          item.key === "0-0"
        ) {
          return true;
        }
        return `${item.id}` === selectedTreeNode[0];
      }
    },
    [selectedTreeNode, pathParts, docSpace, rootFolderType]
  );
  const getEndOfBlock = React.useCallback(
    (item) => {
      if (docSpace) return false;

      switch (item.key) {
        case "0-3":
        case "0-5":
        case "0-6":
          return true;
        default:
          return false;
      }
    },
    [docSpace]
  );

  const getFolderIcon = React.useCallback((item) => {
    let iconUrl = CatalogFolderReactSvgUrl;

    switch (item.rootFolderType) {
      case FolderType.USER:
        iconUrl = CatalogUserReactSvgUrl;
        break;
      case FolderType.Rooms:
        iconUrl = CatalogFolderReactSvgUrl;
        break;
      case FolderType.Archive:
        iconUrl = CatalogArchiveReactSvgUrl;
        break;
      case FolderType.SHARE:
        iconUrl = CatalogSharedReactSvgUrl;
        break;
      case FolderType.COMMON:
        iconUrl = CatalogPortfolioReactSvgUrl;
        break;
      case FolderType.Favorites:
        iconUrl = CatalogFavoritesReactSvgUrl;
        break;
      case FolderType.Recent:
        iconUrl = CatalogRecentReactSvgUrl;
        break;
      case FolderType.Privacy:
        iconUrl = CatalogPrivateReactSvgUrl;
        break;
      case FolderType.TRASH:
        iconUrl = CatalogTrashReactSvgUrl;
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

      if (
        !draggableItems ||
        draggableItems.find(
          (x) => x.id === item.id && x.isFolder === item.isFolder
        )
      )
        return false;

      const isArchive = draggableItems.find(
        (f) => f.rootFolderType === FolderType.Archive
      );

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
          item.rootFolderType === FolderType.COMMON ||
          (item.rootFolderType === FolderType.TRASH && startDrag && !isArchive)
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
      moveDragItems(destFolderId, title, {
        copy: t("Translations:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      });
    },
    [moveDragItems, t]
  );

  const onRemove = React.useCallback(() => {
    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      deleteFromTrash: t("Translations:DeleteFromTrash"),
      deleteSelectedElem: t("Translations:DeleteSelectedElem"),
      FileRemoved: t("Files:FileRemoved"),
      FolderRemoved: t("Files:FolderRemoved"),
    };

    deleteAction(translations);
  }, [deleteAction]);

  const onEmptyTrashAction = () => {
    isMobile && onHide();
    setEmptyTrashDialogVisible(true);
  };

  const getItems = React.useCallback(
    (data) => {
      const items = data.map((item, index) => {
        const isTrash = item.rootFolderType === FolderType.TRASH;
        const showBadge = emptyTrashInProgress
          ? false
          : item.newItems
          ? item.newItems > 0 && true
          : isTrash && !trashIsEmpty;
        const labelBadge = showBadge ? item.newItems : null;
        const iconBadge = isTrash ? ClearTrashReactSvgUrl : null;

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
            onMoveTo={isTrash ? onRemove : onMoveTo}
            onBadgeClick={isTrash ? onEmptyTrashAction : onBadgeClick}
            showDragItems={showDragItems}
            showBadge={showBadge}
            labelBadge={labelBadge}
            iconBadge={iconBadge}
            folderId={`document_catalog-${FolderNames[item.rootFolderType]}`}
          />
        );
      });

      if (!firstLoad && !isVisitor)
        items.splice(3, 0, <SettingsItem key="settings-item" />);
      if (!isVisitor && !isCollaborator)
        items.splice(3, 0, <AccountsItem key="accounts-item" />);

      if (!isVisitor) items.splice(3, 0, <CatalogDivider key="other-header" />);
      else items.splice(2, 0, <CatalogDivider key="other-header" />);

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
      isAdmin,
      isVisitor,
      firstLoad,
      emptyTrashInProgress,
    ]
  );

  return <>{getItems(data)}</>;
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
      bufferSelection,
      dragging,
      setDragging,
      trashIsEmpty,
      firstLoad,
      startDrag,
    } = filesStore;

    const { startUpload } = uploadDataStore;

    const {
      selectedTreeNode,
      treeFolders,
      myFolderId,
      commonFolderId,
      isPrivacyFolder,
    } = treeFoldersStore;

    const { id, pathParts, rootFolderType } = selectedFolderStore;
    const {
      moveDragItems,
      uploadEmptyFolders,
      deleteAction,
      emptyTrashInProgress,
    } = filesActionsStore;
    const { setEmptyTrashDialogVisible } = dialogsStore;

    return {
      isAdmin: auth.isAdmin,
      isVisitor: auth.userStore.user.isVisitor,
      isCollaborator: auth.userStore.user.isCollaborator,
      myId: myFolderId,
      commonId: commonFolderId,
      isPrivacy: isPrivacyFolder,
      currentId: id,
      showText: auth.settingsStore.showText,
      docSpace: auth.settingsStore.docSpace,
      pathParts,
      data: treeFolders,
      selectedTreeNode,
      draggableItems: dragging
        ? bufferSelection
          ? [bufferSelection]
          : selection
        : null,
      dragging,
      setDragging,
      moveDragItems,
      deleteAction,
      startUpload,
      uploadEmptyFolders,
      setEmptyTrashDialogVisible,
      trashIsEmpty,
      rootFolderType,
      firstLoad,
      startDrag,
      emptyTrashInProgress,
    };
  }
)(withTranslation(["Files", "Common", "Translations"])(observer(Items)));
