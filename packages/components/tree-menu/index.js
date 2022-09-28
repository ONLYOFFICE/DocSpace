/* eslint-disable react/prop-types */
import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Tree from "rc-tree";
import "rc-tree/assets/index.css";
import Badge from "../badge";
import Base from "../themes/base";

const StyledTree = styled(Tree)`
  .rc-tree-list-holder-inner {
    .disable-node {
      span.rc-tree-node-content-wrapper {
        pointer-events: none;
        span.rc-tree-iconEle {
          svg {
            path {
              fill: ${(props) => props.theme.treeNode.disableColor};
            }
          }
        }
        span.rc-tree-title {
          color: ${(props) => props.theme.treeNode.disableColor} !important;
        }
      }
      span.rc-tree-switcher {
        pointer-events: none;
        svg {
          path {
            fill: ${(props) => props.theme.treeNode.disableColor};
          }
        }
      }
    }

    .disable-folder {
      span.rc-tree-node-content-wrapper {
        pointer-events: none;
        span.rc-tree-iconEle {
          svg {
            path {
              fill: ${(props) => props.theme.treeNode.disableColor};
            }
          }
        }
        span.rc-tree-title {
          color: ${(props) => props.theme.treeNode.disableColor} !important;
        }
      }
    }

    .rc-tree-treenode {
      height: 36px;
      display: flex;
      align-items: center;
      padding-left: 16px;
      span.rc-tree-switcher {
        ${(props) => props.switcherIcon != null && "background: none"};

        margin-right: 10px;
        vertical-align: 1px;
        height: 24px;
        min-width: 8px;
        max-width: 9px;
        margin-top: -5px;
      }
      span.rc-tree-node-content-wrapper {
        width: 83%;
        span.rc-tree-iconEle {
          margin-right: 8px;
          vertical-align: 5px;
        }
        span.rc-tree-title {
          width: calc(100% - 32px);
          font-weight: 600;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
          color: ${(props) => props.theme.treeNode.title.color};
        }
      }
      .rc-tree-node-selected {
        background: none !important;
        box-shadow: none !important;
        opacity: 1 !important;
      }
    }
    .rc-tree-treenode-selected {
      background: ${(props) => props.theme.treeNode.background};
    }
    .rc-tree-treenode-disabled > span:not(.rc-tree-switcher) {
      color: ${(props) => props.theme.treeMenu.disabledColor} !important;
    }
    .node-motion {
      transition: all 0.3s;
      overflow-y: hidden;
      overflow-x: hidden;
    }
  }
`;
StyledTree.defaultProps = { theme: Base };
const TreeMenu = React.forwardRef((props, ref) => {
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
    theme,
    childrenCount,
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
                fontSize="11px"
                fontWeight={800}
                borderRadius="11px"
                padding="0 5px"
                lineHeight="1.46"
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

  const motion = {
    motionName: "node-motion",
    motionAppear: false,
    onAppearStart: () => ({ height: 0 }),
    onAppearActive: (node) => ({ height: node.scrollHeight }),
    onLeaveStart: (node) => ({ height: node.offsetHeight }),
    onLeaveActive: () => ({ height: 0 }),
  };

  return (
    <>
      <StyledTree
        motion={motion}
        style={style}
        ref={ref}
        className={`${className} not-selectable`}
        id={id}
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
        widthAdditional={`calc(100% + ${
          (childrenCount ? childrenCount : 1) * 32
        }px)`}
        multiplicationFactor={32}
      >
        {modifiedChildren}
      </StyledTree>
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
