import React from "react";
import { connect } from "react-redux";
import { utils, TreeMenu, TreeNode, Icons, toastr } from "asc-web-components";
import {
  selectFolder,
  fetchMyFolder,
  fetchSharedFolder,
  fetchCommonFolder,
  fetchProjectsFolder,
  fetchTrashFolder
} from "../../../store/files/actions";
import { getRootFolders } from "../../../store/files/selectors";
import store from "../../../store/store";

const getItems = data => {
  return data.map(item => {
    if (item.children && item.children.length) {
      return (
        <TreeNode
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <Icons.CatalogFolderIcon size="scale" isfill color="#657077" />
            ) : (
              ""
            )
          }
        >
          {getItems(item.children)}
        </TreeNode>
      );
    }
    return (
      <TreeNode
        key={item.key}
        title={item.title}
        icon={<Icons.CatalogFolderIcon size="scale" isfill color="#657077" />}
      />
    );
  });
};

class ArticleBodyContent extends React.Component {
  shouldComponentUpdate(nextProps) {
    const { selectedKeys, data } = this.props;
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, selectedKeys)) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, data)) {
      return true;
    }

    return false;
  }

  onSelect = data => {
    console.log("onSelect document", data);

    const currentModuleId = Number(data[0]);
    const { currentModule, rootFolders } = this.props;
    if (currentModule !== currentModuleId) {
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

  render() {
    const { data, selectedKeys, fakeNewDocuments } = this.props;

    //console.log("FilesTreeMenu", this.props);

    return data.map((item, index) => (
      <TreeMenu
        key={`TreeMenu_${index}`}
        className="files-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon
        defaultExpandAll
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
        badgeLabel={fakeNewDocuments}
        onBadgeClick={() => console.log("onBadgeClick")}
      >
        {getItems(item)}
      </TreeMenu>
    ));
  }
}

function mapStateToProps(state) {
  const { rootFolders, selectedFolder } = state.files;
  const currentFolderId = selectedFolder.id.toString();
  const fakeNewDocuments = 8;

  return {
    data: getRootFolders(state.files),
    selectedKeys: state.files.selectedFolder ? [currentFolderId] : [""],
    fakeNewDocuments,
    currentModule: currentFolderId,
    rootFolders
  };
}

export default connect(mapStateToProps, {
  selectFolder,
  fetchMyFolder,
  fetchSharedFolder,
  fetchCommonFolder,
  fetchProjectsFolder,
  fetchTrashFolder
})(ArticleBodyContent);
