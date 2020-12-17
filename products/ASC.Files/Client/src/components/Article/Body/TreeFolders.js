import React from "react";
import { TreeMenu, TreeNode, Icons } from "asc-web-components";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { api, constants, toastr, store as initStore } from "asc-web-common";
import { connect } from "react-redux";
import {
  setFilter,
  setTreeFolders,
  setDragItem,
  setDragging,
  setIsLoading,
  setUpdateTree,
} from "../../../store/files/actions";
import {
  getTreeFolders,
  getFilter,
  getDragging,
  getUpdateTree,
  getSelectedFolderId,
  getMyFolderId,
  getShareFolderId,
  getRootFolderId,
  getDraggableItems,
} from "../../../store/files/selectors";
import { onConvertFiles } from "../../../helpers/files-converter";
const { isAdmin } = initStore.auth.selectors;

const { files } = api;
const { FolderType, ShareAccessRights } = constants;

const backgroundDragColor = "#EFEFB2";
const backgroundDragEnterColor = "#F8F7BF";

const StyledTreeMenu = styled(TreeMenu)`
  .rc-tree-node-content-wrapper {
    background: ${(props) => !props.dragging && "none !important"};
  }

  .rc-tree-node-selected {
    background: #dfe2e3 !important;
  }

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

class TreeFolders extends React.Component {
  constructor(props) {
    super(props);

    const { data, expandedKeys } = props;
    this.state = { treeData: data, expandedKeys };
  }

  componentDidUpdate(prevProps) {
    const { expandedKeys, data, needUpdate } = this.props;
    if (
      needUpdate &&
      expandedKeys &&
      this.state.expandedKeys.length !== expandedKeys.length
    ) {
      this.setState({ expandedKeys });
    }

    if (!equal(prevProps.data, data)) {
      //!utils.array.isArrayEqual(prevProps.data, data)) {
      this.setState({ treeData: data });
    }

    if (this.props.updateTree) {
      this.props.setUpdateTree(false);
      this.forceUpdate();
    }
  }

  onBadgeClick = (e) => {
    const id = e.currentTarget.dataset.id;
    this.props.onBadgeClick && this.props.onBadgeClick(id);
  };

  getFolderIcon = (item) => {
    if (item.parentId !== 0)
      return <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />;

    switch (item.rootFolderType) {
      case FolderType.USER:
        return <Icons.CatalogUserIcon size="scale" isfill color="#657077" />;
      case FolderType.SHARE:
        return <Icons.CatalogSharedIcon size="scale" isfill color="#657077" />;
      case FolderType.COMMON:
        return (
          <Icons.CatalogPortfolioIcon size="scale" isfill color="#657077" />
        );
      case FolderType.Favorites:
        return (
          <Icons.CatalogFavoritesIcon size="scale" isfill color="#657077" />
        );
      case FolderType.Recent:
        return <Icons.CatalogRecentIcon size="scale" isfill color="#657077" />;
      case FolderType.Privacy:
        return (
          <Icons.CatalogPrivateRoomIcon size="scale" isfill color="#657077" />
        );

      case FolderType.TRASH:
        return <Icons.CatalogTrashIcon size="scale" isfill color="#657077" />;

      default:
        return <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />;
    }
  };

  showDragItems = (item) => {
    const {
      isAdmin,
      myId,
      commonId,
      rootFolderId,
      currentId,
      draggableItems,
    } = this.props;
    if (item.id === currentId) {
      return false;
    }

    if (draggableItems.find((x) => x.id === item.id)) return false;

    const isMy = rootFolderId === FolderType.USER;
    const isCommon = rootFolderId === FolderType.COMMON;
    const isShare = rootFolderId === FolderType.SHARE;

    if (
      item.rootFolderType === FolderType.SHARE &&
      item.access === ShareAccessRights.FullAccess
    ) {
      return true;
    }

    if (isAdmin) {
      if (isMy || isCommon || isShare) {
        if (
          (item.pathParts &&
            (item.pathParts[0] === myId || item.pathParts[0] === commonId)) ||
          item.rootFolderType === FolderType.USER ||
          item.rootFolderType === FolderType.COMMON
        ) {
          return true;
        }
      }
    } else {
      if (isMy || isCommon || isShare) {
        if (
          (item.pathParts && item.pathParts[0] === myId) ||
          item.rootFolderType === FolderType.USER
        ) {
          return true;
        }
      }
    }

    return false;
  };

  getItems = (data) => {
    return data.map((item) => {
      const dragging = this.props.dragging ? this.showDragItems(item) : false;

      const showBadge = item.newItems
        ? item.newItems > 0 && this.props.needUpdate
        : false;

      if (item.folders && item.folders.length > 0) {
        return (
          <TreeNode
            id={item.id}
            key={item.id}
            title={item.title}
            icon={this.getFolderIcon(item)}
            dragging={dragging}
            newItems={item.newItems}
            onBadgeClick={this.onBadgeClick}
            showBadge={showBadge}
          >
            {this.getItems(item.folders)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          id={item.id}
          key={item.id}
          title={item.title}
          needTopMargin={item.key === "0-5" ? true : false}
          dragging={dragging}
          isLeaf={item.foldersCount ? false : true}
          icon={this.getFolderIcon(item)}
          newItems={item.newItems}
          onBadgeClick={this.onBadgeClick}
          showBadge={showBadge}
        />
      );
    });
  };

  switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }

    if (obj.expanded) {
      return <Icons.ExpanderDownIcon size="scale" isfill color="dimgray" />;
    } else {
      return <Icons.ExpanderRightIcon size="scale" isfill color="dimgray" />;
    }
  };

  loop = (data, curId, child, level) => {
    //if (level < 1 || curId.length - 3 > level * 2) return;
    data.forEach((item) => {
      const itemId = item.id.toString();
      if (curId.indexOf(itemId) >= 0) {
        const listIds = curId;
        const treeItem = listIds.find((x) => x.toString() === itemId);
        if (treeItem === undefined) {
          listIds.push(itemId);
        }
        if (item.folders) {
          this.loop(item.folders, listIds, child);
        } else {
          item.folders = child;
        }
      }
    });
  };

  getNewTreeData(treeData, curId, child, level) {
    this.loop(treeData, curId, child, level);
    this.setLeaf(treeData, curId, level);
  }

  setLeaf(treeData, curKey, level) {
    const loopLeaf = (data, lev) => {
      const l = lev - 1;
      data.forEach((item) => {
        if (
          item.key.length > curKey.length
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
    const folderId = treeNode.props.id;
    let arrayFolders;

    const newFilter = this.props.filter.clone();
    newFilter.filterType = 2;
    newFilter.withSubfolders = null;
    newFilter.authorType = null;

    return files
      .getFolder(folderId, newFilter)
      .then((data) => {
        arrayFolders = data.folders;

        let listIds = [];
        for (let item of data.pathParts) {
          listIds.push(item.toString());
        }

        const folderIndex = treeNode.props.pos;
        let i = 0;
        for (let item of arrayFolders) {
          item["key"] = `${folderIndex}-${i}`;
          i++;
        }

        return { folders: arrayFolders, listIds };
      })
      .catch((err) => toastr.error("Something went wrong", err));
  };

  onLoadData = (treeNode) => {
    this.props.setIsLoading && this.props.setIsLoading(true);
    //console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode)
      .then((data) => {
        const itemId = treeNode.props.id.toString();
        const listIds = data.listIds;
        listIds.push(itemId);

        const treeData = [...this.state.treeData];
        this.getNewTreeData(treeData, listIds, data.folders, 10);
        this.props.needUpdate && this.props.setTreeFolders(treeData);
        this.setState({ treeData });
      })
      .catch((err) => toastr.error(err))
      .finally(() => this.props.setIsLoading && this.props.setIsLoading(false));
  };

  onExpand = (data, treeNode) => {
    if (treeNode.node && !treeNode.node.props.children) {
      if (treeNode.expanded) {
        this.onLoadData(treeNode.node);
      }
    }
    if (this.props.needUpdate) {
      const newFilter = this.props.filter.clone();
      newFilter.treeFolders = data;
      this.props.setFilter(newFilter);
    }

    this.setState({ expandedKeys: data });
  };

  onMouseEnter = (data) => {
    if (this.props.dragging) {
      if (data.node.props.dragging) {
        this.props.setDragItem(data.node.props.id);
      }
    }
  };

  onMouseLeave = () => {
    if (this.props.dragging) {
      this.props.setDragItem(null);
    }
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
    setDragging(false);
    if (dragging) {
      const promise = new Promise((resolve) =>
        onConvertFiles(data.event, resolve)
      );
      promise.then((files) => onTreeDrop(files, id));
    }
  };

  render() {
    const {
      selectedKeys,
      isLoading,
      onSelect,
      needUpdate,
      dragging,
    } = this.props;
    const { treeData, expandedKeys } = this.state;
    const loadProp = needUpdate ? { loadData: this.onLoadData } : {};

    return (
      <StyledTreeMenu
        className="files-tree-menu"
        checkable={false}
        draggable
        disabled={isLoading}
        multiple={false}
        showIcon
        switcherIcon={this.switcherIcon}
        onSelect={onSelect}
        selectedKeys={selectedKeys}
        {...loadProp}
        expandedKeys={expandedKeys}
        onExpand={this.onExpand}
        onMouseEnter={this.onMouseEnter}
        onMouseLeave={this.onMouseLeave}
        onDragOver={this.onDragOver}
        onDragLeave={this.onDragLeave}
        onDrop={this.onDrop}
        dragging={dragging}
        gapBetweenNodes="22"
        gapBetweenNodesTablet="26"
        isFullFillSelection={false}
      >
        {this.getItems(treeData)}
      </StyledTreeMenu>
    );
  }
}

TreeFolders.defaultProps = {
  selectedKeys: [],
  needUpdate: true,
};

function mapStateToProps(state) {
  return {
    treeFolders: getTreeFolders(state),
    filter: getFilter(state),
    myId: getMyFolderId(state),
    commonId: getShareFolderId(state),
    currentId: getSelectedFolderId(state),
    isAdmin: isAdmin(state),
    dragging: getDragging(state),
    updateTree: getUpdateTree(state),
    rootFolderId: getRootFolderId(state),
    draggableItems: getDraggableItems(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setFilter: (filter) => dispatch(setFilter(filter)),
    setTreeFolders: (treeFolders) => dispatch(setTreeFolders(treeFolders)),
    setDragItem: (dragItem) => dispatch(setDragItem(dragItem)),
    setDragging: (dragging) => dispatch(setDragging(dragging)),
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
    setUpdateTree: (updateTree) => dispatch(setUpdateTree(updateTree)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(TreeFolders);
