import React from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import CatalogItem from "@appserver/components/catalog-item";
import { FolderType, ShareAccessRights } from "@appserver/common/constants";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";

const Items = ({
  t,
  data,
  showText,
  pathParts,
  selectedTreeNode,
  onClick,
  onBadgeClick,
  selection,
  setIsLoading,
  isLoading,
  dragging,
  isAdmin,
  myId,
  commonId,
  currentId,
  draggableItems,
  setDragging,
  setStartDrag,
  moveDragItems,
}) => {
  const isActive = (item) => {
    if (selectedTreeNode.length > 0) {
      if (pathParts && pathParts.includes(item.id)) return true;
      if (selectedTreeNode[0] === "@my" && item.key === "0-0") return true;
      return `${item.id}` === selectedTreeNode[0];
    }
  };
  const getEndOfBlock = (item) => {
    switch (item.key) {
      case "0-3":
      case "0-5":
      case "0-6":
        return true;
      default:
        return false;
    }
  };

  const getFolderIcon = (item) => {
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
  };

  const showDragItems = (item) => {
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
  };

  const onMoveTo = (destFolderId, title) => {
    moveDragItems(destFolderId, title, null, {
      copy: t("Translations:CopyOperation"),
      move: t("Translations:MoveToOperation"),
    });
  };

  const getItem = (data) => {
    const items = data.map((item) => {
      const showBadge = item.newItems ? item.newItems > 0 && true : false;

      const isDragging = dragging ? showDragItems(item) : false;

      let value = "";
      if (isDragging) value = `${item.id} dragging`;

      return (
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
          value={value}
          showBadge={showBadge}
          labelBadge={showBadge ? item.newItems : null}
          onClickBadge={onBadgeClick}
        />
      );
    });
    return items;
  };

  return <>{getItem(data)}</>;
};

Items.propTypes = {
  data: PropTypes.array,
  showText: PropTypes.bool,
  selectedTreeNode: PropTypes.array,
  onClick: PropTypes.func,
  onClickBadge: PropTypes.func,
};

export default inject(
  ({
    auth,
    treeFoldersStore,
    selectedFolderStore,
    filesStore,
    filesActionsStore,
  }) => {
    const {
      selection,
      setIsLoading,
      isLoading,
      dragging,
      setDragging,
      setStartDrag,
    } = filesStore;

    const {
      selectedTreeNode,
      treeFolders,
      setTreeFolders,
      myFolderId,
      commonFolderId,
      isPrivacyFolder,
      expandedKeys,
      setExpandedKeys,
      setExpandedPanelKeys,
      getSubfolders,
    } = treeFoldersStore;

    const { id } = selectedFolderStore;

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
      setIsLoading,
      isLoading,
      dragging,
      setDragging,
      setStartDrag,
      moveDragItems: filesActionsStore.moveDragItems,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    withLoader(observer(Items))(<Loaders.DocumentCatalogFolderLoader />)
  )
);
