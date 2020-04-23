import React from "react";
import { TreeMenu, TreeNode, Icons, toastr, utils } from "asc-web-components";
import { fetchFiles } from "../../../store/files/actions";
import store from "../../../store/store";
import { api } from "asc-web-common";
const { files } = api;

class TreeFolders extends React.Component {
  constructor(props) {
    super(props);

    const treeData = props.data;

    this.state = { treeData, expandedKeys: this.props.expandedKeys };

    this.ref = React.createRef();
  }

  getItems = data => {
    return data.map(item => {
      if (item.folders && item.folders.length > 0) {
        return (
          <TreeNode
            id={item.id}
            key={item.id}
            title={item.title}
            icon={
              <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />
            }
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
          isLeaf={item.foldersCount ? false : true}
          icon={<Icons.CatalogFolderIcon size="scale" isfill color="#657077" />}
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

  onSelect = data => {
    if (this.props.selectedKeys[0] !== data[0]) {
      const newFilter = this.props.filter.clone();
      fetchFiles(data[0], newFilter, store.dispatch).catch(err =>
        toastr.error("Something went wrong", err)
      );
    }

    //this.props.selectFolder(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
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
    this.props.onLoading(true);
    //console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode)
      .then(data => {
        const itemId = treeNode.props.id.toString();
        const listIds = data.listIds;
        listIds.push(itemId);

        const treeData = [...this.state.treeData];
        this.getNewTreeData(treeData, listIds, data.folders, 10);
        this.props.setTreeFolders(treeData);
        this.setState({ treeData });
      })
      .catch(() => this.props.onLoading(false))
      .finally(() => this.props.onLoading(false));
  };

  onExpand = data => {
    const newFilter = this.props.filter;
    newFilter.treeFolders = data;

    this.props.setFilter(newFilter);
    this.setState({ expandedKeys: data });
  };

  componentDidUpdate(prevProps) {
    const { expandedKeys, data } = this.props;
    if (this.state.expandedKeys.length !== expandedKeys.length) {
      this.setState({ expandedKeys });
    }

    if (!utils.array.isArrayEqual(prevProps.data, data)) {
      this.setState({ treeData: data });
    }
  }

  render() {
    const { selectedKeys, fakeNewDocuments, isLoading } = this.props;
    const { treeData, expandedKeys } = this.state;

    return (
      <TreeMenu
        ref={this.ref}
        className="files-tree-menu"
        checkable={false}
        draggable={false}
        disabled={isLoading}
        multiple={false}
        showIcon
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
        badgeLabel={fakeNewDocuments}
        onBadgeClick={() => console.log("onBadgeClick")}
        loadData={this.onLoadData}
        expandedKeys={expandedKeys}
        onExpand={this.onExpand}
      >
        {this.getItems(treeData)}
      </TreeMenu>
    );
  }
}

export default TreeFolders;
