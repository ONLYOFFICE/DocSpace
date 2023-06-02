import React, { useState } from "react";
import TreeMenu from "../tree-menu";
import TreeNode from "../tree-menu/sub-components/tree-node";
import CatalogDepartmentsReactSvg from "PUBLIC_DIR/images/catalog.departments.react.svg";
import CatalogFolderReactSvg from "PUBLIC_DIR/images/catalog.folder.react.svg";
import ExpanderDownReactSvg from "PUBLIC_DIR/images/expander-down.react.svg";
import ExpanderRightReactSvg from "PUBLIC_DIR/images/expander-right.react.svg";

export default {
  title: "Examples/Tree",
  component: TreeMenu,
  subcomponents: { TreeNode },
  parameters: { docs: { description: { component: "Example" } } },
  argTypes: {
    data: { table: { disable: true } },
    disableSwitch: { description: "Disables Switch" },
  },
};

const treeData = [
  {
    key: "0-0",
    title: "Departments",
    root: true,
    children: [
      { key: "0-0-0", title: "Development", root: false },
      { key: "0-0-1", title: "Direction", root: false },
      { key: "0-0-2", title: "Marketing", root: false },
      { key: "0-0-3", title: "Mobile", root: false },
      { key: "0-0-4", title: "Support", root: false },
      { key: "0-0-5", title: "Web", root: false },
    ],
  },
];

const Template = (args) => {
  // eslint-disable-next-line react/prop-types
  const { data } = args;

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

  const onDrop = (info) => {
    info.event.persist();
    const dropKey = info.node.props.eventKey;
    const dragKey = info.dragNode.props.eventKey;
    const dropPos = info.node.props.pos.split("-");
    const dropPosition =
      info.dropPosition - Number(dropPos[dropPos.length - 1]);

    const loop = (data, key, callback) => {
      // eslint-disable-next-line react/prop-types
      data.forEach((item, index, arr) => {
        if (item.key === key) {
          callback(item, index, arr);
          return;
        }
        if (item.children) {
          loop(item.children, key, callback);
        }
      });
    };
    const data = [...gData];

    let dragObj;
    loop(data, dragKey, (item, index, arr) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      loop(data, dropKey, (item) => {
        item.children = item.children || [];
        item.children.push(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 &&
      info.node.props.expanded &&
      dropPosition === 1
    ) {
      loop(data, dropKey, (item) => {
        item.children = item.children || [];
        item.children.unshift(dragObj);
      });
    } else {
      let ar;
      let i;
      loop(data, dropKey, (item, index, arr) => {
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
  const onExpand = (expandedKeys) => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  };

  const loop = (data) => {
    // eslint-disable-next-line react/prop-types
    return data.map((item) => {
      if (item.children && item.children.length) {
        return (
          <TreeNode
            title={item.title}
            key={item.key}
            icon={
              item.root ? (
                <CatalogDepartmentsReactSvg size="scale" color="dimgray" />
              ) : (
                ""
              )
            }
          >
            {loop(item.children)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          key={item.key}
          title={item.title}
          icon={
            !item.root ? (
              <CatalogFolderReactSvg size="scale" color="dimgray" />
            ) : (
              ""
            )
          }
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
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon={true}
        defaultExpandAll={false}
        defaultExpandParent={true}
        onExpand={onExpand}
        autoExpandParent={autoExpandParent}
        expandedKeys={expandedKeys}
        defaultExpandedKeys={["0-0"]}
        defaultSelectedKeys={["0-0-2"]}
        onDragStart={(info) => onDragStart(info)}
        onDrop={(info) => onDrop(info)}
        onDragEnter={onDragEnter}
        switcherIcon={switcherIcon}
      >
        {loop(gData)}
      </TreeMenu>
    </div>
  );
};

export const peopleTreeMenu = Template.bind({});
peopleTreeMenu.args = {
  data: treeData,
};
