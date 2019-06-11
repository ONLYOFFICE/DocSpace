import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

const StyledDropdown = styled.div`
    position: absolute;
    top: 100%;
    left: 0;
    z-index: 1000;
    margin-top: 4px;
    display: ${props => (props.isOpen || props.opened ? 'block' : 'none')};
    padding: 4px;
    border-radius: 4px;
    -moz-border-radius: 4px;
    -webkit-border-radius: 4px;
    background: white;
    border: 1px solid #d1d1d1;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    -moz-box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    -webkit-box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
`;

const StyledDropdownItem = styled.button`
    background: none;
    border: 0;
    color: #333333;
    cursor: pointer;
    display: block;
    font-size: 12px;
    line-height: 24px;
    margin: 0;
    padding: 0 12px;
    text-decoration: none;
    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;
    white-space: nowrap;
    -moz-user-select: none;
    -ms-user-select: none;
    -o-user-select: none;
    -webkit-user-select: none;
    stroke: none;

    &:hover{
        background-color: #e9e9e9;
        border-radius: 2px;
        -moz-border-radius: 2px;
        -webkit-border-radius: 2px;
    }
`;

const DropDown = props => {
    return (
        <StyledDropdown {...props}>
        {React.Children.map(props.children, (child) =>
            <StyledDropdownItem 
                tabIndex={child.props.tabIndex || -1} 
                onClick={child.props.clickAction || child.props.onClick}
            >
                {child.props.text || child.props.label}
            </StyledDropdownItem>)}
    </StyledDropdown>
    );
}

export default DropDown