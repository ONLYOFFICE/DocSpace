/* eslint-disable react/prop-types */
import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Tree from "rc-tree";

import Badge from "../badge";
import Text from "../text";

import { tablet } from "../utils/device";

const StyledTreeMenu = styled(Tree)`
  margin: 0;
  padding: 0;
  width: 93%;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media ${tablet} {
    width: 90%;
  }

  .rc-tree-switcher {
    margin-left: 0 !important;
    margin-top: 1px;
  }

  & li span.rc-tree-iconEle {
    margin-left: 3px;
    width: 18px;
    height: 16px;
    padding: 0;
    margin-top: 4px;
  }

  ${(props) =>
    props.isEmptyRootNode &&
    css`
      & > li > span.rc-tree-switcher-noop {
        display: none;
      }
    `}
  span.rc-tree-switcher {
    margin-right: 6px !important;
  }
  .rc-tree-node-content-wrapper {
    position: static !important;
    margin-bottom: ${(props) => +props.gapBetweenNodes - 16 + "px;"};
  }

  ${(props) =>
    !props.isFullFillSelection &&
    css`
      span.rc-tree-node-selected {
        width: min-content !important;
        padding-right: 4px;
        max-width: 98%;
      }
    `}

  & .rc-tree-node-selected .rc-tree-title {
    ${(props) => !props.isFullFillSelection && "width: calc(100% - 26px);"}
  }

  &:not(.rc-tree-show-line) .rc-tree-switcher-noop {
    background: none;
  }
  &.rc-tree-show-line li:not(:last-child) > ul {
    background: url("data:image/gif;base64,R0lGODlhCQACAIAAAMzMzP///yH5BAEAAAEALAAAAAAJAAIAAAIEjI9pUAA7")
      0 0 repeat-y;
  }
  &.rc-tree-show-line li:not(:last-child) > .rc-tree-switcher-noop {
    background-position: -56px -18px;
  }
  &.rc-tree-show-line li:last-child > .rc-tree-switcher-noop {
    background-position: -56px -36px;
  }
  .rc-tree-child-tree {
    display: none;
  }
  .rc-tree-treenode-switcher-open {
    ${(props) => props.disableSwitch && "margin-bottom:10px;"}
  }
  .rc-tree-child-tree-open {
    display: block;
    ${(props) => props.disableSwitch && "margin: 0 0 25px 0;"}
    margin-left: ${(props) => (props.disableSwitch ? "27px" : "8px")};
    li:first-child {
      margin-top: ${(props) => (props.disableSwitch ? "10px" : "6px")};
      margin-left: 0;
    }
  }
  .rc-tree-treenode-disabled > span:not(.rc-tree-switcher),
  .rc-tree-treenode-disabled > a,
  .rc-tree-treenode-disabled > a span {
    color: #767676;
    cursor: not-allowed;
  }

  .rc-tree-icon__open {
    margin-right: 2px;
    background-position: -110px -16px;
    vertical-align: top;
  }
  .rc-tree-icon__close {
    margin-right: 2px;
    background-position: -110px 0;
    vertical-align: top;
  }
  .rc-tree-icon__docu {
    margin-right: 2px;
    background-position: -110px -32px;
    vertical-align: top;
  }
  .rc-tree-icon__customize {
    margin-right: 2px;
    vertical-align: top;
  }
  ${(props) =>
    props.switcherIcon != null
      ? css`
          li span.rc-tree-switcher {
            background: none;
          }
        `
      : ""}
  ${(props) =>
    props.disableSwitch
      ? css`
          li span.rc-tree-switcher {
            height: 0;
            margin: 0;
            width: 0;
          }
        `
      : ``}
  @media (max-width: 1024px) {
    margin-top: 20px !important;
    .rc-tree-node-content-wrapper {
      margin-bottom: ${(props) =>
        props.gapBetweenNodesTablet
          ? +props.gapBetweenNodesTablet - 16 + "px;"
          : +props.gapBetweenNodes - 16 + "px;"};
    }
    & > li > .rc-tree-child-tree {
      margin-left: 4px;
    }
  }
`;

const TreeMenu = React.forwardRef((props, ref) => {
  //console.log("TreeMenu render");
  const {
    defaultExpandAll,
    defaultExpandParent,
    showIcon,
    showLine,
    multiple,
    disabled,
    draggable,
    dragging,
    checkable,
    children,
    switcherIcon,
    icon,
    onDragStart,
    onDrop,
    onSelect,
    onDragEnter,
    onDragEnd,
    onDragLeave,
    onDragOver,
    onCheck,
    onExpand,
    onLoad,
    onMouseEnter,
    onMouseLeave,
    onRightClick,
    defaultSelectedKeys,
    expandedKeys,
    defaultExpandedKeys,
    defaultCheckedKeys,
    selectedKeys,
    className,
    id,
    style,
    loadData,
    disableSwitch,
    isFullFillSelection,
    gapBetweenNodes,
    gapBetweenNodesTablet,
    isEmptyRootNode,
  } = props;
  const expandedKeysProp = expandedKeys ? { expandedKeys: expandedKeys } : {};

  const onTreeNodeSelect = (data, e) => {
    const result = e.selected ? data : [e.node.props.eventKey];
    onSelect(result, e);
  };

  const renderChildren = (children) => {
    const items = [];

    React.Children.forEach(children, (child, i) => {
      if (child.props.newItems > 0 && child.props.showBadge) {
        const el = React.cloneElement(child, {
          icon: (
            <>
              {child.props.icon}
              <Badge
                data-id={child.props.id}
                className="newItem"
                key={child.props.id + "-badge"}
                label={child.props.newItems}
                backgroundColor="#ED7309"
                color="#FFF"
                fontSize="11px"
                fontWeight={800}
                borderRadius="11px"
                padding="0 5px"
                onClick={child.props.onBadgeClick}
              />
            </>
          ),
        });
        items.push(el);
      } else {
        items.push(child);
      }
    });
    return items;
  };

  const modifiedChildren = renderChildren(children);

  return (
    <>
      <StyledTreeMenu
        id={id}
        style={style}
        className={`${className} not-selectable`}
        ref={ref}
        {...expandedKeysProp}
        loadData={loadData}
        checkable={!!checkable}
        draggable={!!draggable}
        dragging={dragging}
        disabled={!!disabled}
        multiple={!!multiple}
        showLine={!!showLine}
        showIcon={!!showIcon}
        defaultExpandAll={!!defaultExpandAll}
        defaultExpandParent={!!defaultExpandParent}
        icon={icon}
        selectedKeys={selectedKeys}
        defaultSelectedKeys={defaultSelectedKeys}
        defaultExpandedKeys={defaultExpandedKeys}
        defaultCheckedKeys={defaultCheckedKeys}
        onDragStart={onDragStart}
        onDrop={onDrop}
        onDragEnd={onDragEnd}
        onDragLeave={onDragLeave}
        onDragOver={onDragOver}
        switcherIcon={switcherIcon}
        onSelect={onTreeNodeSelect}
        onDragEnter={onDragEnter}
        onCheck={onCheck}
        onExpand={onExpand}
        onLoad={onLoad}
        onMouseEnter={onMouseEnter}
        onMouseLeave={onMouseLeave}
        onRightClick={onRightClick}
        disableSwitch={disableSwitch}
        isFullFillSelection={isFullFillSelection}
        gapBetweenNodes={gapBetweenNodes}
        gapBetweenNodesTablet={gapBetweenNodesTablet}
        isEmptyRootNode={isEmptyRootNode}
      >
        {modifiedChildren}
      </StyledTreeMenu>
    </>
  );
});

TreeMenu.propTypes = {
  /** Whether support checked  */
  checkable: PropTypes.bool,
  /** Whether can drag treeNode */
  draggable: PropTypes.bool,
  /** Whether disabled the tree */
  disabled: PropTypes.bool,
  /** Whether multiple select */
  multiple: PropTypes.bool,
  /** Whether show icon  */
  showIcon: PropTypes.bool,
  /** Whether show line */
  showLine: PropTypes.bool,
  /** Expand all treeNodes */
  defaultExpandAll: PropTypes.bool,
  /** Auto expand parent treeNodes when init  */
  defaultExpandParent: PropTypes.bool,

  icon: PropTypes.func,
  /** it execs when fire the tree's dragend event */
  onDragEnd: PropTypes.func,
  /** it execs when fire the tree's dragenter event  */
  onDragEnter: PropTypes.func,
  /** it execs when fire the tree's dragleave event */
  onDragLeave: PropTypes.func,
  /** it execs when fire the tree's dragover event  */
  onDragOver: PropTypes.func,
  /** it execs when fire the tree's dragstart event */
  onDragStart: PropTypes.func,
  /** it execs when fire the tree's drop event  */
  onDrop: PropTypes.func,
  /** fire on treeNode expand or not */
  onExpand: PropTypes.func,
  /** Trigger when a node is loaded. If you set the loadedKeys, you must handle onLoad to avoid infinity loop */
  onLoad: PropTypes.func,
  /** call when mouse enter a treeNode */
  onMouseEnter: PropTypes.func,
  /** call when mouse leave a treeNode */
  onMouseLeave: PropTypes.func,
  /** select current treeNode and show customized contextmenu */
  onRightClick: PropTypes.func,
  /** click the treeNode to fire  */
  onSelect: PropTypes.func,
  /** load data asynchronously and the return value should be a promise  */
  loadData: PropTypes.func,
  /** child elements */
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),

  disableSwitch: PropTypes.bool,
  /** to select the selection style of the active node */
  isFullFillSelection: PropTypes.bool,
  /** for setting the spacing between nodes */
  gapBetweenNodes: PropTypes.string,
  /** to set spacing between nodes on tablets and phones (if necessary) */
  gapBetweenNodesTablet: PropTypes.string,
  /** swipe the root node to the left if there are no nested elements */
  isEmptyRootNode: PropTypes.bool,
};

TreeMenu.defaultProps = {
  disableSwitch: false,
  isFullFillSelection: true,
  gapBetweenNodes: "15",
  isEmptyRootNode: false,
};

TreeMenu.displayName = "TreeMenu";

export default TreeMenu;
