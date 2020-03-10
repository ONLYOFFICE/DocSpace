import React from "react";
import { TreeMenu, TreeNode, Icons, toastr } from "asc-web-components";
import { fetchFolder } from "../../../store/files/actions";
import store from "../../../store/store";
import { api, history } from "asc-web-common";
const { files } = api;

class TreeFolders extends React.Component {
  constructor(props) {
    super(props);

    const treeData = props.data;

    this.state = { treeData };

    this.ref = React.createRef();
  }

  getItems = data => {
    return data.map((item, index) => {
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
      fetchFolder(data[0], store.dispatch).catch(() =>
        toastr.error("Something went wrong")
      );
    }

    //this.props.selectFolder(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  };

  loop = (data, curId, child, level) => {
    //if (level < 1 || curId.length - 3 > level * 2) return;
    data.forEach(item => {
      if (curId.indexOf(item.id) >= 0) {
        const { filter, setFilter } = this.props;
        const newFilter = filter.clone();
        const treeItem = newFilter.treeFolders.find(x => x === item.id);
        if (treeItem === undefined) {
          newFilter.treeFolders.push(item.id);
        }
        setFilter(newFilter);
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

    return files
      .getFolder(folderId)
      .then(data => {
        arrayFolders = data.folders;
        let i = 0;
        for (let item of arrayFolders) {
          item["key"] = `${folderIndex}-${i}`;
          i++;
        }
        return arrayFolders;
      })
      .catch(() => toastr.error("Something went wrong"));
  };

  onLoadData = treeNode => {
    //console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode).then(folders => {
      let listId;
      if (this.props.filter.treeFolders.length === 0) {
        listId = [treeNode.props.id];
      } else {
        const newFilter = this.props.filter;
        newFilter.treeFolders.push(treeNode.props.id);
        listId = newFilter.treeFolders;
      }
      const treeData = [...this.state.treeData];
      this.getNewTreeData(treeData, listId, folders, 10);
      this.setState({ treeData });
    });
  };

  componentDidUpdate(prevProps) {
    if (this.props.defaultExpandedKeys !== prevProps.defaultExpandedKeys) {
      this.ref.current.setState({
        expandedKeys: this.props.defaultExpandedKeys
      });
    }
  }

  render() {
    const { selectedKeys, fakeNewDocuments, defaultExpandedKeys } = this.props;
    const { treeData } = this.state;

    //console.log("TreeFolders render", this.props);
    return (
      <TreeMenu
        ref={this.ref}
        className="files-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon
        defaultExpandedKeys={defaultExpandedKeys}
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
        badgeLabel={fakeNewDocuments}
        onBadgeClick={() => console.log("onBadgeClick")}
        loadData={this.onLoadData}
      >
        {this.getItems(treeData)}
      </TreeMenu>
    );
  }
}

export default TreeFolders;
