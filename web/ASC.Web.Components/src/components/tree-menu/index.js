/* eslint-disable react/prop-types */
import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import Tree from 'rc-tree';
import Badge from "../badge";

const StyledTreeContainer = styled.div`
    display: flex;

    .tree_bage {
        display: inline-table;
        z-index: 1;
        overflow: unset;
        padding: 3px;
    }
`;

const StyledTreeMenu = styled(Tree)`
    margin: 0;
    padding: 0;
    width: 93%;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    
    &:not(.rc-tree-show-line) .rc-tree-switcher-noop {
        background: none;
    }
    &.rc-tree-show-line li:not(:last-child) > ul {
        background: url('data:image/gif;base64,R0lGODlhCQACAIAAAMzMzP///yH5BAEAAAEALAAAAAAJAAIAAAIEjI9pUAA7') 0 0 repeat-y;
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
    .rc-tree-child-tree-open {
        display: block;
        margin-left: 8px;
        li:first-child{
            margin-top: 6px;
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
    ${props => props.switcherIcon != null ?
        css`
            li span.rc-tree-switcher{
                background: none;
            }
        `
        : ''
    }
    /* .rc-tree-title {
        width: ${props => props.badgeLabel && 'calc(100% - 60px) !important'};
    } */
  
`;

const TreeMenu = React.forwardRef((props, ref) => {
    //console.log("TreeMenu render");
    const { defaultExpandAll, defaultExpandParent, showIcon, showLine, multiple, disabled, draggable, checkable, children, switcherIcon, icon,
        onDragStart, onDrop, onSelect, onDragEnter, onDragEnd, onDragLeave, onDragOver, onCheck, onExpand, onLoad, onMouseEnter, onMouseLeave, onRightClick,
        defaultSelectedKeys, expandedKeys, defaultExpandedKeys, defaultCheckedKeys, selectedKeys, className, id, style, badgeLabel, onBadgeClick, loadData } = props;

    const expandedKeysProp = expandedKeys ? { expandedKeys: expandedKeys } : {};

    const onTreeNodeSelect = (data, e) => {
        const result = e.selected ? data : [e.node.props.eventKey];
        onSelect(result, e);
    }
    return (
        <StyledTreeContainer
            className={className}
            id={id}
            style={style}
        >
            <StyledTreeMenu
                ref={ref}
                {...expandedKeysProp}
                loadData={loadData}
                checkable={!!checkable}
                draggable={!!draggable}
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

                badgeLabel={badgeLabel}
            >
                {children}
            </StyledTreeMenu>
                {badgeLabel && (                
                    <Badge
                        className="tree_bage"
                        label={badgeLabel}
                        backgroundColor="#ED7309"
                        color="#FFFFFF"
                        fontSize="11px"
                        fontWeight={800}
                        borderRadius="11px"
                        padding="0 5px"
                        maxWidth="50px"
                        onClick={onBadgeClick}
                    />
                )}
        </StyledTreeContainer>
    );
})

TreeMenu.propTypes = {
    checkable: PropTypes.bool,
    draggable: PropTypes.bool,
    disabled: PropTypes.bool,
    multiple: PropTypes.bool,
    showIcon: PropTypes.bool,
    showLine: PropTypes.bool,
    defaultExpandAll: PropTypes.bool,
    defaultExpandParent: PropTypes.bool,

    icon: PropTypes.func,
    onDragStart: PropTypes.func,
    onDrop: PropTypes.func,
    onBadgeClick: PropTypes.func,
    loadData: PropTypes.func,

    children: PropTypes.oneOfType([
        PropTypes.arrayOf(PropTypes.node),
        PropTypes.node
    ]),
    className: PropTypes.string,
    id: PropTypes.string,
    style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
    badgeLabel: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
}

TreeMenu.displayName = "TreeMenu";

export default TreeMenu