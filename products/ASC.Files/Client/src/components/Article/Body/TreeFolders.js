import React from "react";
import { TreeMenu, TreeNode, Icons, toastr } from "asc-web-components";
import {
  fetchMyFolder,
  fetchSharedFolder,
  fetchCommonFolder,
  fetchProjectsFolder,
  fetchTrashFolder,
  setFilter
} from "../../../store/files/actions";
import store from "../../../store/store";

import { api } from "asc-web-common";
const { files } = api;

class TreeFolders extends React.Component {
  constructor(props) {
    super(props);

    const treeData = props.data;

    this.state = { treeData };
  }

  getItems = (data, key) => {
    return data.map((item, index) => {
      const newKey = key ? key : 0;
      if (item.folders && item.folders.length > 0) {
        return (
          <TreeNode
            id={item.id}
            key={`${newKey}-${index}`}
            title={item.title}
            //isLeaf={item.isLeaf}
            icon={
              <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />
            }
          >
            {this.getItems(item.folders, item.key)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          id={item.id}
          key={`${newKey}-${index}`}
          title={item.title}
          isLeaf={item.isLeaf}
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
    console.log("onSelect document", data);

    const currentModuleId = Number(data[0]);
    const { currentModule, rootFolders } = this.props;

    if (currentModule !== data[0]) {
      const { my, share, common, project, trash } = rootFolders;

      switch (currentModuleId) {
        case my.id:
          fetchMyFolder(store.dispatch).catch(() =>
            toastr.error("Error fetchMyFolder")
          );
          break;
        case share.id:
          fetchSharedFolder(store.dispatch).catch(() =>
            toastr.error("Error fetchSharedFolder")
          );
          break;
        case common.id:
          fetchCommonFolder(store.dispatch).catch(() =>
            toastr.error("Error fetchCommonFolder")
          );
          break;
        case project.id:
          fetchProjectsFolder(store.dispatch).catch(() =>
            toastr.error("Error fetchProjectsFolder")
          );

          break;
        case trash.id:
          fetchTrashFolder(store.dispatch).catch(() =>
            toastr.error("Error fetchTrashFolder")
          );
          break;
        default:
          break;
      }
    }
    //this.props.selectFolder(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  };

  onExpand = data => {
    console.log("onExpand", data);

    const { state, filter } = this.props;

    const newFilter = filter.clone();
    //array[array.length - 1]
    //data[0] && newFilter.folderPath.push(data[0]);
    //setFilter(newFilter);

    const lastItem = data[data.length - 1];

    const currentModuleId = Number(lastItem);
    const myId = state.files.rootFolders["my"].id;
    const shareId = state.files.rootFolders["share"].id;
    const commonId = state.files.rootFolders["common"].id;
    const projectId = state.files.rootFolders["project"].id;
    const trashId = state.files.rootFolders["trash"].id;

    switch (currentModuleId) {
      case myId:
        newFilter.myPath.push(lastItem); //remove dublicate // sort
        console.log("myId", filter.myPath);
        break;
      case shareId:
        console.log("shareId", filter.sharedPath);
        break;
      case commonId:
        console.log("commonId", filter.commonPath);
        break;
      case projectId:
        console.log("projectId", filter.projectPath);
        break;
      case trashId:
        console.log("trashId", filter.recycleBinPath);
        break;
      default:
        console.log("default");
        break;
    }
  };

  getNewTreeData(treeData, curKey, child, level) {
    console.log("child", child);
    const loop = data => {
      if (level < 1 || curKey.length - 3 > level * 2) return;
      data.forEach(item => {
        if (curKey.indexOf(item.key) === 0) {
          if (item.folders) {
            loop(item.folders);
          } else {
            item.folders = child;
          }
        }
      });
    };
    loop(treeData);
    this.setLeaf(treeData, curKey, level);
  }

  setLeaf(treeData, curKey, level) {
    const loopLeaf = (data, lev) => {
      console.log("treeDatatreeDatatreeData", data);
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

    return files.getFolder(folderId).then(data => {
      arrayFolders = data.folders;
      let i = 0;
      for (let item of arrayFolders) {
        item["key"] = `${folderIndex}-${i}`;
        i++;
      }
      return arrayFolders;
    });
  };

  onLoadData = treeNode => {
    console.log("load data...", treeNode);

    return this.generateTreeNodes(treeNode).then(folders => {
      const treeData = [...this.state.treeData];
      this.getNewTreeData(treeData, treeNode.props.eventKey, folders, 10);
      this.setState({ treeData });
    });
  };

  render() {
    const { selectedKeys, fakeNewDocuments } = this.props;
    const { treeData } = this.state;

    //console.log("Tree files render", this.props);
    return (
      <TreeMenu
        className="files-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
        badgeLabel={fakeNewDocuments}
        onBadgeClick={() => console.log("onBadgeClick")}
        //onExpand={this.onExpand}
        loadData={this.onLoadData}
      >
        {this.getItems(treeData)}
      </TreeMenu>
    );
  }
}

export default TreeFolders;
