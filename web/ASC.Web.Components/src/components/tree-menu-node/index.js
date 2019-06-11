import React from 'react'
import styled , { css } from 'styled-components';
import PropTypes from 'prop-types'
import {TreeNode} from 'rc-tree';


const TreeNodeMenu = styled(TreeNode)`
    &.rc-tree-treenode-selected::after {
        content: '';
        background-color: #e9e9e9;
        top: -2px;
        left: 0;
        width: 100%;
        height: ${props => (props.size == 'big' ? '29px' : '25px')};
        position: absolute;
        z-index: -1;
    }
    .rc-tree-node-selected .rc-tree-title{
        color: #0857a6;
        font-weight: normal;
    }
    .rc-tree-node-selected .rc-tree-title:hover{
        text-decoration: ${props => (props.isUnderline ? 'underline' : 'none')};
    }
    .rc-tree-node-content-wrapper:hover{
        text-decoration: ${props => (props.isUnderline ? 'underline' : 'none')};
    }
    ul li{
        padding: 0 0 0 26px;
        margin: 7px 0 0 0;
    }
    
    position: relative;
    font-size: ${props => (props.size == 'big' ? '15px' : props.size == 'middle' ? '12px' : '10px')};
    font-family: "Open Sans", sans-serif;
    
`;

TreeNodeMenu.propTypes = {
    size: PropTypes.oneOf(['big', 'middle']),
    isUnderline: PropTypes.bool
  };
  
  TreeNodeMenu.defaultProps = {
    size: 'big',
    isUnderline: true
  };

export default TreeNodeMenu;