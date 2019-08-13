import React, { useCallback } from 'react';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { selectGroup } from '../../../store/people/actions';

const getItems = data => {
  return data.map(item => {
    if (item.children && item.children.length) {
      return (
        <TreeNode
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <Icons.CatalogDepartmentsIcon
                size="scale"
                isfill={true}
                color="dimgray"
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
              isfill={true}
              color="dimgray"
            />
          ) : (
              ""
            )
        }
      />
    );
  });
};

const PeopleTreeMenu = props => {
  const { data, selectGroup, defaultSelectedKeys } = props;

  console.log("PeopleTreeMenu", props);

  const onSelect = useCallback(data => {
    selectGroup(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  }, [selectGroup])

  const switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon size="scale" isfill={true} color="dimgray" />
      );
    } else {
      return (
        <Icons.ExpanderRightIcon size="scale" isfill={true} color="dimgray" />
      );
    }
  };

  return (
    <TreeMenu
      checkable={false}
      draggable={false}
      disabled={false}
      multiple={false}
      showIcon={true}
      defaultExpandAll={true}
      switcherIcon={switcherIcon}
      onSelect={onSelect}
      defaultSelectedKeys={defaultSelectedKeys}
    >
      {getItems(data)}
    </TreeMenu>
  );
};

const ArticleBodyContent = props => <PeopleTreeMenu {...props} />;

const getTreeGroups = (groups) => {
  const treeData = [
      {
          key: "root",
          title: "Departments",
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
  return {
    data: getTreeGroups(state.people.groups),
    defaultSelectedKeys: state.people.selectedGroup ? [state.people.selectedGroup] : ["root"]
  };
}

export default connect(mapStateToProps, { selectGroup })(ArticleBodyContent);