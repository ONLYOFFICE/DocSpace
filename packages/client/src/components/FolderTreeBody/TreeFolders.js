import React from "react";
import TreeMenu from "@docspace/components/tree-menu";
import TreeNode from "@docspace/components/tree-menu/sub-components/tree-node";
import styled from "styled-components";
import { FolderType, ShareAccessRights } from "@docspace/common/constants";
import { onConvertFiles } from "../../helpers/files-converter";
import { ReactSVG } from "react-svg";
import ExpanderDownIcon from "PUBLIC_DIR/images/expander-down.react.svg";
import ExpanderRightIcon from "PUBLIC_DIR/images/expander-right.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { observer, inject } from "mobx-react";
import { runInAction } from "mobx";
import { withTranslation } from "react-i18next";
import Base from "@docspace/components/themes/base";

const backgroundDragColor = "#EFEFB2";
const backgroundDragEnterColor = "#F8F7BF";

const StyledTreeMenu = styled(TreeMenu)`
  width: 100%;
  .rc-tree-node-content-wrapper {
    background: ${(props) => !props.dragging && "none !important"};
  }

  /* .rc-tree-node-selected {
    background: ${(props) =>
    !props.isPanel
      ? props.theme.filesArticleBody.background
      : props.theme.filesArticleBody.panelBackground} !important;
  } */

  .rc-tree-treenode-disabled > span:not(.rc-tree-switcher),
  .rc-tree-treenode-disabled > a,
  .rc-tree-treenode-disabled > a span {
    cursor: wait;
  }
  /*
  span.rc-tree-iconEle {
    margin-left: 4px;
  }*/
`;

StyledTreeMenu.defaultProps = { theme: Base };

const StyledFolderSVG = styled.div`
  svg {
    width: 100%;

    path {
      fill: ${(props) => props.theme.filesArticleBody.fill};
    }
  }
`;

StyledFolderSVG.defaultProps = { theme: Base };

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesArticleBody.expanderColor};
  }
`;
StyledExpanderDownIcon.defaultProps = { theme: Base };

const StyledExpanderRightIcon = styled(ExpanderRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesArticleBody.expanderColor};
  }
`;
StyledExpanderRightIcon.defaultProps = { theme: Base };

class TreeFolders extends React.Component {
  constructor(props) {
    super(props);

    this.state = { isExpand: false, isLoading: false };
    this.selectionFoldersId = [];
  }

  setItemSecurityRights = (data) => {
    const { selectedKeys, setItemSecurity } = this.props;
    const selectedFolder = data.find(
      (x) => x.id.toString() === selectedKeys[0]
    );

    selectedFolder && setItemSecurity(selectedFolder.security);
  };
  componentDidMount() {
    const { selectionFiles, expandedPanelKeys = [], data } = this.props;
    this.props.isLoadingNodes && this.props.setIsLoadingNodes(false);

    const isRootFolder = expandedPanelKeys.length === 0;

    isRootFolder && this.setItemSecurityRights(data);

    if (selectionFiles) {
      for (let item of selectionFiles) {
        if (!item.fileExst) {
          this.selectionFoldersId.push(item.id);
        }
      }
    }
  }

  onBadgeClick = (e) => {
    e.stopPropagation();
    const id = e.currentTarget.dataset.id;
    this.props.onBadgeClick && this.props.onBadgeClick(id);
  };

  getFolderIcon = (item) => {
    let iconUrl = "/static/images/catalog.folder.react.svg";

    switch (item.rootFolderType) {
      case FolderType.USER:
        iconUrl = "/static/images/catalog.user.react.svg";
        break;
      case FolderType.SHARE:
        iconUrl = "/static/images/catalog.share.react.svg";
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

    if (item.parentId !== 0)
      iconUrl = "/static/images/catalog.folder.react.svg";

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
        iconUrl = "/static/images/cloud.services.kdrive.react.svg";
        break;
      case "Yandex":
        iconUrl = "/static/images/cloud.services.yandex.react.svg";
        break;
      case "NextCloud":
        iconUrl = "/static/images/cloud.services.nextcloud.react.svg";
        break;
      case "OwnCloud":
        iconUrl = "/static/images/catalog.folder.react.svg";
        break;
      case "WebDav":
        iconUrl = "/static/images/cloud.services.webdav.react.svg";
        break;
      default:
        break;
    }

    return (
      <StyledFolderSVG>
        <ReactSVG src={iconUrl} />
      </StyledFolderSVG>
    );
  };

  showDragItems = (item) => {
    const { isAdmin, myId, commonId, currentId, draggableItems } = this.props;
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

  getItems = (props) => {
    const { data, path } = props;

    return data.map((item) => {
      const dragging = this.props.dragging ? this.showDragItems(item) : false;
      const showBadge = false;
      const provider = item.providerKey;
      const serviceFolder = !!item.providerKey;

      let value = "",
        disableNodeValue = "";
      if (dragging) value = `${item.id} dragging ${provider}`;

      const { roomsFolderId, expandedPanelKeys } = this.props;

      let isDisabledNode = false;
      if (item.id == roomsFolderId) {
        isDisabledNode = expandedPanelKeys?.includes(roomsFolderId + "");
      }

      if (this.selectionFoldersId && this.selectionFoldersId.includes(item.id))
        disableNodeValue = "disable-node";

      if (isDisabledNode) disableNodeValue += " disable-folder ";

      if ((item.folders && item.folders.length > 0) || serviceFolder) {
        return (
          <TreeNode
            id={item.id}
            key={item.id}
            className={`tree-drag ${item.folderClassName} ${disableNodeValue}`}
            data-value={value}
            title={item.title}
            icon={this.getFolderIcon(item)}
            dragging={dragging}
            isLeaf={
              item.rootFolderType === FolderType.Privacy &&
              !this.props.isDesktop
                ? true
                : null
            }
            newItems={
              !this.props.isDesktop &&
              item.rootFolderType === FolderType.Privacy
                ? null
                : item.newItems
            }
            providerKey={item.providerKey}
            onBadgeClick={this.onBadgeClick}
            showBadge={showBadge}
            path={path}
            security={item.security}
          >
            {item.rootFolderType === FolderType.Privacy && !this.props.isDesktop
              ? null
              : this.getItems({
                  data: item.folders ? item.folders : [],
                  path: [...path, { id: item.id, title: item.title }],
                })}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          id={item.id}
          key={item.id}
          className={`tree-drag ${item.folderClassName} ${disableNodeValue}`}
          data-value={value}
          title={item.title}
          needTopMargin={item.rootFolderType === FolderType.TRASH}
          dragging={dragging}
          isLeaf={
            (item.rootFolderType === FolderType.Privacy &&
              !this.props.isDesktop) ||
            !item.foldersCount
              ? true
              : false
          }
          icon={this.getFolderIcon(item)}
          newItems={
            !this.props.isDesktop && item.rootFolderType === FolderType.Privacy
              ? null
              : item.newItems
          }
          providerKey={item.providerKey}
          onBadgeClick={this.onBadgeClick}
          showBadge={showBadge}
          security={item.security}
        />
      );
    });
  };

  switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <StyledExpanderDownIcon theme={this.props.theme} size="scale" />;
    } else {
      return <StyledExpanderRightIcon theme={this.props.theme} size="scale" />;
    }
  };

  loop = (data, child, pos) => {
    let newPos = pos.split("-");
    let newData = data;

    newPos.shift();
    while (newPos.length !== 1) {
      const index = +newPos[0];
      newData = newData[index].folders;
      newPos.shift();
    }

    runInAction(() => {
      newData[newPos].folders = child;
    });
  };

  getNewTreeData(treeData, curId, child, pos) {
    const { selectedNodeParentId, setIsLoadingNodes } = this.props;
    !this.expand && selectedNodeParentId && setIsLoadingNodes(true);

    this.loop(treeData, child, pos);
    this.setLeaf(treeData, curId, 10);
  }

  setLeaf(treeData, curKey, level) {
    const loopLeaf = (data, lev) => {
      const l = lev - 1;
      data.forEach((item) => {
        if (
          item.key?.length > curKey.length
            ? item.key.indexOf(curKey) !== 0
            : curKey.indexOf(item.key) !== 0
        ) {
          return;
        }
        if (item.folders) {
          loopLeaf(item.folders, l);
        } else if (l < 1) {
          item.isLeaf = true;
        }
      });
    };
    loopLeaf(treeData, level + 1);
  }

  generateTreeNodes = (treeNode) => {
    const { withoutProvider } = this.props;
    const folderId = treeNode.id;
    const level = treeNode.pos;

    let arrayFolders, proverIndex;
    return this.props.getSubfolders(folderId).then((data) => {
      arrayFolders = data;

      const folderIndex = treeNode.pos;
      let i = 0;

      for (let item of arrayFolders) {
        item["key"] = `${folderIndex}-${i}`;

        if (withoutProvider && item.providerKey) {
          proverIndex = i;
        }

        i++;
      }

      if (proverIndex) {
        arrayFolders.splice(proverIndex, 1);
      }

      return { folders: arrayFolders, listIds: [], level };
    });
  };

  onLoadData = (treeNode, isExpand) => {
    const {
      data: incomingDate,
      certainFolders,
      roomsFolderId,
      expandedPanelKeys,
    } = this.props;
    isExpand && this.setState({ isExpand: true });
    //console.log("load data...", treeNode);

    this.setState({
      isLoading: true,
    });

    if (this.state.isExpand && !isExpand) {
      return Promise.resolve();
    }

    return this.generateTreeNodes(treeNode)
      .then((data) => {
        const itemId = treeNode.id.toString();
        const listIds = data.listIds;
        listIds.push(itemId);

        const treeData = certainFolders
          ? [...incomingDate]
          : [...this.props.treeFolders];

        this.getNewTreeData(treeData, listIds, data.folders, data.level);
        !certainFolders && this.props.setTreeFolders(treeData);

        const isLastFoldersLevel =
          expandedPanelKeys[expandedPanelKeys.length - 1] === data.listIds[0];

        isLastFoldersLevel && this.setItemSecurityRights(data.folders);

        if (
          data.listIds[0] == roomsFolderId &&
          this.props.onSelect &&
          this.selectedRootRoom
        ) {
          const roomsIndex = treeData.findIndex((f) => f.id == roomsFolderId);
          const firstRoomsNodeId = treeData[roomsIndex]?.folders[0]?.id;

          this.selectedRootRoom = false;

          treeData[roomsIndex]?.folders[0] &&
            this.props.onSelect(
              [firstRoomsNodeId],
              treeData[roomsIndex]?.folders[0]
            );
        }
      })
      .catch((err) => console.log("err", err))
      .finally(() => {
        this.setState({ isExpand: false, isLoading: false });
      });
  };

  onExpand = (expandedKeys, treeNode, isRoom = false) => {
    this.expand = true;
    if (treeNode.node && !treeNode.node.children) {
      if (treeNode.expanded) {
        this.onLoadData(treeNode.node, true);
      }
    } else if (isRoom) {
      this.props.onSelect(
        [treeNode.node.children[0].id],
        treeNode.node.children[0]
      );
    }

    this.props.setExpandedPanelKeys(expandedKeys);
  };

  onSelect = (folder, treeNode) => {
    const { onSelect, expandedPanelKeys = [], roomsFolderId } = this.props;

    const newExpandedPanelKeys = JSON.parse(JSON.stringify(expandedPanelKeys));
    newExpandedPanelKeys.push(folder[0]);

    if (folder[0] == roomsFolderId) {
      this.onExpand(newExpandedPanelKeys, treeNode, true);
      this.selectedRootRoom = true;
      return;
    }

    onSelect && onSelect(folder, treeNode.node);
  };

  onDragOver = (data) => {
    const parentElement = data.event.target.parentElement;
    const existElement = parentElement.classList.contains(
      "rc-tree-node-content-wrapper"
    );

    if (existElement) {
      if (data.node.props.dragging) {
        parentElement.style.background = backgroundDragColor;
      }
    }
  };

  onDragLeave = (data) => {
    const parentElement = data.event.target.parentElement;
    const existElement = parentElement.classList.contains(
      "rc-tree-node-content-wrapper"
    );

    if (existElement) {
      if (data.node.props.dragging) {
        parentElement.style.background = backgroundDragEnterColor;
      }
    }
  };

  onDrop = (data) => {
    const { setDragging, onTreeDrop } = this.props;
    const { dragging, id } = data.node.props;
    //if (dragging) {
    setDragging(false);
    const promise = new Promise((resolve) =>
      onConvertFiles(data.event, resolve)
    );
    promise.then((files) => onTreeDrop(files, id));
    //}
  };
  onLoad = (loadedKeys, options) => {
    const { firstLoadScroll, selectedNodeParentId } = this.props;
    //console.log("onLoad tree nodes", "loadedKeys", treeNode, "options", options);

    if (
      !this.expand &&
      selectedNodeParentId &&
      loadedKeys.includes(selectedNodeParentId.toString())
    ) {
      firstLoadScroll();
    }
  };

  render() {
    const {
      selectedKeys,
      dragging,
      expandedPanelKeys,
      treeFolders,
      data,
      disabled,
      isPanel,
      isLoadingNodes,
    } = this.props;

    const { isLoading } = this.state;

    return (
      <StyledTreeMenu
        isPanel={isPanel}
        className="files-tree-menu"
        checkable={false}
        draggable={dragging}
        disabled={isLoadingNodes || isLoading || disabled}
        multiple={false}
        showIcon
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
        loadData={this.onLoadData}
        expandedKeys={expandedPanelKeys}
        onExpand={this.onExpand}
        onDragOver={this.onDragOver}
        onDragLeave={this.onDragLeave}
        onDrop={this.onDrop}
        dragging={dragging}
        gapBetweenNodes="22"
        gapBetweenNodesTablet="26"
        isFullFillSelection={false}
        childrenCount={expandedPanelKeys?.length}
        onLoad={this.onLoad}
      >
        {this.getItems(
          { data: data, path: [] } || { data: treeFolders, path: [] }
        )}
      </StyledTreeMenu>
    );
  }
}

TreeFolders.defaultProps = {
  selectedKeys: [],
};

export default inject(
  (
    {
      auth,
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      selectFolderDialogStore,
    },
    { useDefaultSelectedKeys, selectedKeys }
  ) => {
    const { selection, dragging, setDragging } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      myFolderId,
      commonFolderId,
      setExpandedPanelKeys,
      getSubfolders,
      setIsLoadingNodes,
      isLoadingNodes,
      roomsFolderId,
    } = treeFoldersStore;
    const { id, parentId: selectedNodeParentId } = selectedFolderStore;
    const { setItemSecurity } = selectFolderDialogStore;
    return {
      setItemSecurity,
      isAdmin: auth.isAdmin,
      isDesktop: auth.settingsStore.isDesktopClient,
      dragging,
      currentId: id,
      myId: myFolderId,
      commonId: commonFolderId,
      draggableItems: dragging ? selection : null,
      treeFolders,
      selectedKeys: useDefaultSelectedKeys
        ? treeFoldersStore.selectedKeys
        : selectedKeys,

      setDragging,
      setTreeFolders,
      setExpandedPanelKeys,
      getSubfolders,
      setIsLoadingNodes,
      isLoadingNodes,
      selectedNodeParentId,
      roomsFolderId,
    };
  }
)(withTranslation(["Files", "Common"])(observer(TreeFolders)));
