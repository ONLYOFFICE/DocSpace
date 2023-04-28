import React, { useState } from "react";
import TreeMenu from ".";
import TreeNode from "./sub-components/tree-node";
import ExpanderDownReactSvg from "PUBLIC_DIR/images/expander-down.react.svg";
import ExpanderRightReactSvg from "PUBLIC_DIR/images/expander-right.react.svg";
import CatalogFolderReactSvg from "PUBLIC_DIR/images/catalog.folder.react.svg";

const treeData = [
  {
    key: "0-0",
    children: [{ key: "0-0-0" }, { key: "0-0-1" }],
  },
];

export default {
  title: "Components/TreeMenu",
  component: TreeMenu,
  argTypes: {
    disableSwitch: {
      description: "Disables Switch",
      control: "action",
    },
    showBadge: {
      description: "Displays the badge",
    },
    newItems: {
      description: "The number of new elements in the node",
    },
    title: {
      description: "Title of the subTree",
    },
    data: {
      description:
        "Tree object that contains a key that facilitates building the tree structure",
    },
  },
};

const Template = ({
  data,
  title,
  newItems,
  showBadge,
  onSelect,
  onLoad,
  onCheck,
  onRightClick,
  ...args
}) => {
  const [gData, setGData] = useState(data);
  const [autoExpandParent, setAutoExpandParent] = useState(true);
  const [expandedKeys, setExpandedKeys] = useState([
    "0-0-key",
    "0-0-0-key",
    "0-0-0-0-key",
  ]);

  const onDragStart = (info) => {
    info.event.persist();
  };

  const onDragEnter = (info) => {
    setExpandedKeys(info.expandedKeys);
  };

  const onBadgeClick = (e) => {
    const id = e.currentTarget.dataset.id;
    console.log("Clocked on badge: ", id);
  };

  const onDrop = (info) => {
    info.event.persist();
    const dropKey = info.node.props.eventKey;
    const dragKey = info.dragNode.props.eventKey;
    const dropPos = info.node.props.pos.split("-");
    const dropPosition =
      info.dropPosition - Number(dropPos[dropPos.length - 1]);

    const loop = (treeData, key, callback) => {
      treeData.forEach((item, index, arr) => {
        if (item.key === key) {
          callback(item, index, arr);
          return;
        }
        if (item.children) {
          loop(item.children, key, callback);
        }
      });
    };

    const treeData = [...gData];

    let dragObj;
    loop(treeData, dragKey, (item, index, arr) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      loop(treeData, dropKey, (item) => {
        item.children = item.children || [];
        item.children.push(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 &&
      info.node.props.expanded &&
      dropPosition === 1
    ) {
      loop(treeData, dropKey, (item) => {
        item.children = item.children || [];
        item.children.unshift(dragObj);
      });
    } else {
      let ar;
      let i;
      loop(treeData, dropKey, (item, index, arr) => {
        ar = arr;
        i = index;
      });
      if (dropPosition === -1) {
        ar.splice(i, 0, dragObj);
      } else {
        ar.splice(i + 1, 0, dragObj);
      }
    }
    setGData(treeData);
  };

  const onExpand = (expandedKeys) => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  };

  const getTreeNodes = (tree) => {
    return tree.map((item) => {
      if (item.children && item.children.length) {
        return (
          <TreeNode
            title={title}
            key={item.key}
            icon={
              <CatalogFolderReactSvg
                size="scale"
                //isfill=true,
                color="dimgray"
              />
            }
            onBadgeClick={onBadgeClick}
            newItems={newItems}
            showBadge={showBadge}
          >
            {getTreeNodes(item.children)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          key={item.key}
          title={title}
          icon={React.createElement(CatalogFolderReactSvg, {
            size: "scale",
            //isfill: true,
            color: "dimgray",
          })}
        ></TreeNode>
      );
    });
  };

  const switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <ExpanderDownReactSvg width="8px" color="dimgray" />;
    } else {
      return <ExpanderRightReactSvg width="8px" color="dimgray" />;
    }
  };

  return (
    <div style={{ width: "250px", margin: "20px" }}>
      <TreeMenu
        {...args}
        onExpand={onExpand}
        autoExpandParent={autoExpandParent}
        expandedKeys={expandedKeys}
        onDragStart={(info) => onDragStart(info)}
        onDrop={(info) => onDrop(info)}
        onDragEnter={onDragEnter}
        switcherIcon={switcherIcon}
        onSelect={() => onSelect("select")}
        onLoad={() => onLoad("load")}
        onCheck={() => onCheck("check")}
        onRightClick={() => onRightClick("rightClick")}
      >
        {getTreeNodes(gData)}
      </TreeMenu>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  checkable: false,
  draggable: false,
  disabled: false,
  multiple: false,
  showIcon: true,
  isFullFillSelection: true,
  isEmptyRootNode: false,
  defaultExpandAll: false,
  defaultExpandParent: true,
  data: treeData,
  title: "Title",
  newItems: 0,
  showBadge: false,
};
