import React, { useState } from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  boolean,
  text,
  select,
  number,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import TreeMenu from ".";
import TreeNode from "./sub-components/tree-node";
import { Icons } from "../icons";
import { action } from "@storybook/addon-actions";

const iconNames = Object.keys(Icons);

const treeData = [
  {
    key: "0-0",
    children: [{ key: "0-0-0" }, { key: "0-0-1" }],
  },
];

const TreeMenuStory = (props) => {
  // eslint-disable-next-line react/prop-types
  const { data } = props;

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
            title={text("title", "Title")}
            key={item.key}
            icon={React.createElement(
              Icons[select("icon", iconNames, "CatalogFolderIcon")],
              { size: "scale", isfill: true, color: "dimgray" }
            )}
            onBadgeClick={onBadgeClick}
            newItems={number("newItems", 0)}
            showBadge={boolean("showBadge", false)}
          >
            {getTreeNodes(item.children)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          key={item.key}
          title={text("title", "Title")}
          icon={React.createElement(
            Icons[select("icon", iconNames, "CatalogFolderIcon")],
            { size: "scale", isfill: true, color: "dimgray" }
          )}
        ></TreeNode>
      );
    });
  };

  const switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon
          size="scale"
          isfill={true}
          color="dimgray"
        ></Icons.ExpanderDownIcon>
      );
    } else {
      return (
        <Icons.ExpanderRightIcon
          size="scale"
          isfill={true}
          color="dimgray"
        ></Icons.ExpanderRightIcon>
      );
    }
  };

  return (
    <div style={{ width: "250px", margin: "20px" }}>
      <TreeMenu
        checkable={boolean("checkable", false)}
        draggable={boolean("draggable", false)}
        disabled={boolean("disabled", false)}
        badgeLabel={number("badgeLabel")}
        multiple={boolean("multiple", false)}
        showIcon={boolean("showIcon", true)}
        isFullFillSelection={boolean("isFullFillSelection", true)}
        gapBetweenNodes={text("gapBetweenNodes")}
        gapBetweenNodesTablet={text("gapBetweenNodesTablet")}
        isEmptyRootNode={boolean("isEmptyRootNode", false)}
        defaultExpandAll={boolean("defaultExpandAll", false)}
        defaultExpandParent={boolean("defaultExpandParent", true)}
        onExpand={onExpand}
        autoExpandParent={autoExpandParent}
        expandedKeys={expandedKeys}
        onDragStart={(info) => onDragStart(info)}
        onDrop={(info) => onDrop(info)}
        onDragEnter={onDragEnter}
        switcherIcon={switcherIcon}
        onSelect={action("select")}
        onLoad={action("load")}
        onCheck={action("check")}
        onRightClick={action("rightClick")}
      >
        {getTreeNodes(gData)}
      </TreeMenu>
    </div>
  );
};

storiesOf("Components|Tree", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => <TreeMenuStory data={treeData} />);
