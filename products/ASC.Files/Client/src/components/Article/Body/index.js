import React from 'react';
import { utils } from 'asc-web-components';
import { connect } from 'react-redux';
import {
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
          !item.root ? (
            <Icons.CatalogFolderIcon
              size="scale"
              isfill
              color="#657077"
            />
          ) : (
              ""
            )
        }
      />
    );
  });
};

class ArticleBodyContent extends React.Component {

  shouldComponentUpdate(nextProps) {
    if(!utils.array.isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)) {
      return true;
    }

    if(!utils.array.isArrayEqual(nextProps.data, this.props.data)) {
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

const getTreeGroups = (groups, departments) => {
  const treeData = [
      {
          key: "root",
          title: departments,
          root: true,
          children: groups.map(g => {
              return {
                  key: g.id, title: g.name, root: false
              };
          }) || []
      }
  ];

  return treeData;
};

function mapStateToProps(state) {

  const defaultFolders = ["Мои документы", "Доступно для меня", "Общие документы", "Документы проектов", "Корзина"];
  const fakeFolders = [
    { id: "00000000-0000-0000-0000-000000000001", name: "fakeFolder1", manager: null},
    { id: "00000000-0000-0000-0000-000000000002", name: "fakeFolder2", manager: null},
    { id: "00000000-0000-0000-0000-000000000003", name: "fakeFolder3", manager: null},
    { id: "00000000-0000-0000-0000-000000000004", name: "fakeFolder4", manager: null},
    { id: "00000000-0000-0000-0000-000000000005", name: "fakeFolder5", manager: null}
  ];

  const myDocumentsFolder = getTreeGroups(fakeFolders, defaultFolders[0]);
  const sharedWithMeFolder = getTreeGroups(fakeFolders, defaultFolders[1]);
  const commonDocumentsFolder = getTreeGroups(fakeFolders, defaultFolders[2]);
  const projectDocumentsFolder = getTreeGroups(fakeFolders, defaultFolders[3]);
  const recycleBinFolder = getTreeGroups(fakeFolders, defaultFolders[4]);

  const fakeNewDocuments = 8;

  const data = [myDocumentsFolder, sharedWithMeFolder, commonDocumentsFolder, projectDocumentsFolder, recycleBinFolder];

  return {
    data,
    selectedKeys: state.files.selectedFolder ? [state.files.selectedFolder] : ["root"],
    fakeNewDocuments
  };
}

export default connect(mapStateToProps, { selectFolder })(ArticleBodyContent);