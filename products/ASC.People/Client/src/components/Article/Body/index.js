import React, { useCallback } from 'react';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { getTreeGroups } from '../../../store/people/selectors';
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
  const { data, onGroupSelect } = props;

  const onSelect = useCallback(data => {
    onGroupSelect(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  }, [onGroupSelect])

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
    >
      {getItems(data)}
    </TreeMenu>
  );
};

const ArticleBodyContent = ({ treeData, selectGroup }) => <PeopleTreeMenu data={treeData} onGroupSelect={selectGroup} />;

function mapStateToProps(state) {
  return {
    treeData: getTreeGroups(state.people.groups)
  };
}

export default connect(mapStateToProps, { selectGroup })(ArticleBodyContent);