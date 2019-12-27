import React from 'react';
import { utils } from 'asc-web-components';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { selectGroup } from '../../../store/people/actions';
import { departments } from './../../../helpers/customNames';

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
              isfill={true}
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
    this.props.selectGroup(data && data.length === 1 && data[0] !== "root" ? data[0] : null);
  };

  switcherIcon = obj => {
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

  render() {
    const { data, selectedKeys } = this.props;

    //console.log("PeopleTreeMenu", this.props);

    return (
      <TreeMenu
        className="people-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon={true}
        defaultExpandAll={true}
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
      >
        {getItems(data)}
      </TreeMenu>
    );
  };
};

const getTreeGroups = (groups) => {
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
  return {
    data: getTreeGroups(state.people.groups),
    selectedKeys: state.people.selectedGroup ? [state.people.selectedGroup] : ["root"]
  };
}

export default connect(mapStateToProps, { selectGroup })(ArticleBodyContent);