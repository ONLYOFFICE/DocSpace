import React, { useState } from 'react';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import { getTreeGroups } from '../../../store/people/selectors';

const PeopleTreeMenu = props => {
  const { data } = props;

  const [gData, setGData] = useState(data);
  const [autoExpandParent, setAutoExpandParent] = useState(true);

  const [expandedKeys, setExpandedKeys] = useState([
    "0-0-key",
    "0-0-0-key",
    "0-0-0-0-key"
  ]);

  const onDragStart = info => {
    info.event.persist();
  };

  const onDragEnter = info => {
    setExpandedKeys(info.expandedKeys);
  };

  const onDrop = info => {
    info.event.persist();
    const dropKey = info.node.props.eventKey;
    const dragKey = info.dragNode.props.eventKey;
    const dropPos = info.node.props.pos.split("-");
    const dropPosition =
      info.dropPosition - Number(dropPos[dropPos.length - 1]);

    const getItems = (data, key, callback) => {
      data.forEach((item, index, arr) => {
        if (item.key === key) {
          callback(item, index, arr);
          return;
        }
        if (item.children) {
          getItems(item.children, key, callback);
        }
      });
    };
    const data = [...gData];

    let dragObj;
    getItems(data, dragKey, (item, index, arr) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      getItems(data, dropKey, item => {
        item.children = item.children || [];
        item.children.push(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 &&
      info.node.props.expanded &&
      dropPosition === 1
    ) {
      getItems(data, dropKey, item => {
        item.children = item.children || [];
        item.children.unshift(dragObj);
      });
    } else {
      let ar;
      let i;
      getItems(data, dropKey, (item, index, arr) => {
        ar = arr;
        i = index;
      });
      if (dropPosition === -1) {
        ar.splice(i, 0, dragObj);
      } else {
        ar.splice(i + 1, 0, dragObj);
      }
    }
    setGData(data);
  };
  const onExpand = expandedKeys => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  };

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
      defaultExpandParent={true}
      onExpand={onExpand}
      autoExpandParent={autoExpandParent}
      expandedKeys={expandedKeys}
      onDragStart={info => onDragStart(info)}
      onDrop={info => onDrop(info)}
      onDragEnter={onDragEnter}
      switcherIcon={switcherIcon}
    >
      {getItems(gData)}
    </TreeMenu>
  );
};

const ArticleBodyContent = ({ treeData }) => <PeopleTreeMenu data={treeData} />;

function mapStateToProps(state) {
  return {
    treeData: getTreeGroups(state.people.groups)
  };
}

export default connect(mapStateToProps)(ArticleBodyContent);