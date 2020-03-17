import React from "react";
import { TreeMenu, TreeNode, Icons, toastr, utils } from "asc-web-components";
import { fetchFolder } from "../../../store/files/actions";
import store from "../../../store/store";
import { api, history } from "asc-web-common";
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
      const url = `${history.location.pathname}${history.location.search}`;
      const symbol =
        history.location.hash ||
        history.location.search[history.location.search.length - 1] === "/"
          ? ""
          : "/";
      history.push(`${url}${symbol}#${data[0]}`);
      fetchFolder(data[0], store.dispatch).catch(err =>
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
        const { filter } = this.props;
        const newFilter = filter.clone();
        const treeItem = newFilter.treeFolders.find(
          x => x.toString() === itemId
        );
        if (treeItem === undefined) {
          newFilter.treeFolders.push(itemId);
        }
        if (item.folders) {
          this.loop(item.folders, newFilter.treeFolders, child);
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
    const folderIndex = treeNode.props.pos;
    let arrayFolders;

    const newFilter = this.props.filter.clone();
    newFilter.filterType = 2;

    return files
      .getFolder(folderId, newFilter)
      .then(data => {
        arrayFolders = data.folders;
        let i = 0;
        for (let item of arrayFolders) {
          item["key"] = `${folderIndex}-${i}`;
          i++;
        }
        return arrayFolders;
      })
      .catch(err => toastr.error("Something went wrong", err));
  };

  onLoadData = treeNode => {
    this.props.onLoading(true);
    //console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode)
      .then(folders => {
        let listId;
        const itemId = treeNode.props.id.toString();
        if (this.props.filter.treeFolders.length === 0) {
          listId = [itemId];
        } else {
          listId = this.props.filter.clone();
          if (!listId.treeFolders.includes(itemId)) {
            listId.treeFolders.push(itemId);
          }
          listId = listId.treeFolders;
        }
        const treeData = [...this.state.treeData];
        this.getNewTreeData(treeData, listId, folders, 10);

        this.setState({ treeData });
      })
      .catch(() => this.props.onLoading(false))
      .finally(() => this.props.onLoading(false));
  };

  onExpand = data => {
    this.setState({ expandedKeys: data });
  };
  
  componentDidUpdate(prevProps) {
    if (
      this.state.expandedKeys.length !== this.props.expandedKeys.length
    ) {
      this.setState({ expandedKeys: this.props.expandedKeys });
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
