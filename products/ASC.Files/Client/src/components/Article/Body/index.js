import React from 'react';
import { connect } from 'react-redux';
import {
  utils,
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { selectFolder } from '../../../store/files/actions';

const getItems = data => {
  return data.map(item => {
    if (item.children && item.children.length) {
      return (
        <TreeNode
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <Icons.CatalogFolderIcon
                size="scale"
                isfill
                color="#657077"
              />
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
        icon={
          <Icons.CatalogFolderIcon
            size="scale"
            isfill
            color="#657077"
          />
        }
      />
    );
  });
};

class ArticleBodyContent extends React.Component {

  shouldComponentUpdate(nextProps) {
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, this.props.data)) {
      return true;
    }

    return false;
  }

  onSelect = data => {
    console.log("onSelect folder", data);

    //this.props.selectFolder(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  };

  switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon size="scale" isfill color="dimgray" />
      );
    } else {
      return (
        <Icons.ExpanderRightIcon size="scale" isfill color="dimgray" />
      );
    }
  };

  render() {
    const { data, selectedKeys, fakeNewDocuments } = this.props;

    //console.log("FilesTreeMenu", this.props);
    console.log("data", data);

    return (
      data.map((item, index) =>
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
          onBageClick={() => console.log("onBageClick")}
        >
          {getItems(item)}
        </TreeMenu>
      )
    );
  };
};

const getTreeGroups = (groups, departments, key) => {
  const treeData = [
      {
          key: key,
          title: departments,
          root: true,
          children: groups.map(g => {
              return {
                  key: g.id, title: g.title || g.name, root: false
              };
          }) || []
      }
  ];

  return treeData;
};

function mapStateToProps(state) {

  const defaultFolders = ["Мои документы", "Доступно для меня", "Общие документы", "Документы проектов", "Корзина"];

  //TODO: Get default folder ids
  const getFakeFolders = count => Array.from(Array(count), (x, index) => {
    return {
      id: `00000000-0000-0000-0000-00000000000${index}`,
      name: `fakeFolder${index}`,
      manager: null
    }
  });

  const myDocumentsFolder = getTreeGroups(state.files.folders, state.files.selectedFolder.title, "1");
  const sharedWithMeFolder = getTreeGroups(getFakeFolders(4), defaultFolders[1], "2");
  const commonDocumentsFolder = getTreeGroups(getFakeFolders(state.files.rootFolders.common.foldersCount), state.files.rootFolders.common.title || defaultFolders[2], "3");
  const projectDocumentsFolder = getTreeGroups(getFakeFolders(state.files.rootFolders.project.foldersCount), state.files.rootFolders.project.title || defaultFolders[3], "4");
  const recycleBinFolder = getTreeGroups([], state.files.rootFolders.trash.title || defaultFolders[3], "5");

  const fakeNewDocuments = 8;

  const data = [myDocumentsFolder, sharedWithMeFolder, commonDocumentsFolder, projectDocumentsFolder, recycleBinFolder];

  return {
    data,
    selectedKeys: state.files.selectedFolder ? [state.files.selectedFolder.id.toString()] : [""],
    fakeNewDocuments
  };
}

export default connect(mapStateToProps, { selectFolder })(ArticleBodyContent);