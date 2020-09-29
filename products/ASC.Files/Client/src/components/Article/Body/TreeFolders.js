import React from "react";
import {
  TreeMenu,
  TreeNode,
  Icons,
  utils,
  Badge
} from "asc-web-components";
import styled from "styled-components";
import { api, constants, toastr } from "asc-web-common";
const { files } = api;
const { FolderType, ShareAccessRights } = constants;

const backgroundDragColor = "#EFEFB2";
const backgroundDragEnterColor = "#F8F7BF";

const StyledTreeMenu = styled(TreeMenu)`
  .rc-tree-node-content-wrapper {
    background: ${props => !props.dragging && "none !important"};
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

    const treeData = props.data;
    this.state = { treeData, expandedKeys: props.expandedKeys };
  }

  onBadgeClick = e => {
    const id = e.currentTarget.dataset.id;
    this.props.onBadgeClick && this.props.onBadgeClick(id);
  };

  getFolderIcon = item => {
    const showItem = item.newItems
      ? item.newItems > 0 && this.props.needUpdate
      : false;
    const style = { position: "absolute", right: 1, top: 2 };
    const badgeProps = {
      label: item.newItems,
      backgroundColor: "#ED7309",
      color: "#FFF",
      fontSize: "11px",
      fontWeight: 800,
      borderRadius: "11px",
      padding: "0 5px",
      style
    };

    switch (item.key) {
      case "0-0":
        return <Icons.CatalogUserIcon size="scale" isfill color="#657077" />;

      case "0-1":
        return (
          <>
            <Icons.CatalogSharedIcon size="scale" isfill color="#657077" />
            {showItem && (
              <Badge
                data-id={item.id}
                {...badgeProps}
                onClick={this.onBadgeClick}
              />
            )}
          </>
        );

      case "0-2":
        return (
          <>
            <Icons.CatalogPortfolioIcon size="scale" isfill color="#657077" />
            {showItem && (
              <Badge
                data-id={item.id}
                {...badgeProps}
                onClick={this.onBadgeClick}
              />
            )}
          </>
        );

      case "0-3":
        return <Icons.CatalogTrashIcon size="scale" isfill color="#657077" />;

      case "0-4":
        return <Icons.CatalogFavoritesIcon size="scale" isfill color="#657077" />;

      default:
        return <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />;
    }
  };

  showDragItems = item => {
    const {
      isAdmin,
      myId,
      commonId,
      isCommon,
      isMy,
      isShare,
      currentId
    } = this.props;
    if (item.id === currentId) {
      return false;
    }

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

  getItems = data => {
    return data.map(item => {
      const dragging = this.showDragItems(item);

      if (item.folders && item.folders.length > 0) {
        return (
          <TreeNode
            id={item.id}
            key={item.id}
            title={item.title}
            icon={this.getFolderIcon(item)}
            dragging={this.props.dragging && dragging}
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
          dragging={this.props.dragging && dragging}
          isLeaf={item.foldersCount ? false : true}
          icon={this.getFolderIcon(item)}
        />
      );
    });
  };

  switcherIcon = obj => {
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
    data.forEach(item => {
      const itemId = item.id.toString();
      if (curId.indexOf(itemId) >= 0) {
        const listIds = curId;
        const treeItem = listIds.find(x => x.toString() === itemId);
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
      data.forEach(item => {
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

  generateTreeNodes = treeNode => {
    const folderId = treeNode.props.id;
    let arrayFolders;

    const newFilter = this.props.filter.clone();
    newFilter.filterType = 2;
    newFilter.withSubfolders = null;
    newFilter.authorType = null;

    return files
      .getFolder(folderId, newFilter)
      .then(data => {
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
      .catch(err => toastr.error("Something went wrong", err));
  };

  onLoadData = treeNode => {
    this.props.setIsLoading && this.props.setIsLoading(true);
    //console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode)
      .then(data => {
        const itemId = treeNode.props.id.toString();
        const listIds = data.listIds;
        listIds.push(itemId);

        const treeData = [...this.state.treeData];
        this.getNewTreeData(treeData, listIds, data.folders, 10);
        this.props.needUpdate && this.props.setTreeFolders(treeData);
        this.setState({ treeData });
      })
      .catch(err => toastr.error(err))
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

  componentDidUpdate(prevProps) {
    const { expandedKeys, data, needUpdate } = this.props;
    if (
      needUpdate &&
      expandedKeys &&
      this.state.expandedKeys.length !== expandedKeys.length
    ) {
      this.setState({ expandedKeys });
    }

    if (!utils.array.isArrayEqual(prevProps.data, data)) {
      this.setState({ treeData: data });
    }
  }

  onMouseEnter = data => {
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

  onDragOver = data => {
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

  onDragLeave = data => {
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

  onDrop = data => {
    const { setDragging, onTreeDrop } = this.props;
    const { dragging, id } = data.node.props;
    setDragging(false);
    if (dragging) {
      onTreeDrop(data.event, id);
    }
  };

  render() {
    const {
      selectedKeys,
      isLoading,
      onSelect,
      needUpdate,
      dragging
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
  needUpdate: true
};

export default TreeFolders;
