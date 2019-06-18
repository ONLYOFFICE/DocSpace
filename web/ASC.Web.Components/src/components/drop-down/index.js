import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

const StyledDropdown = styled.div`
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 13px;
    line-height: 36px;

    position: absolute;
    top: 100%;
    left: 0;
    z-index: 1000;
    margin-top: 0px;
    display: ${props => (props.isOpen || props.opened ? 'block' : 'none')};
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    background: #FFFFFF;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const StyledDropdownItem = styled.button`
    width: 100%;
    text-align: left;
    background: none;
    border: 0;
    color: #333333;
    cursor: pointer;
    display: block;
    line-height: 36px;
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
        background-color: #F8F9F9;
        width: 100%;
        text-align: left;

        &:first-of-type{
            border-radius: 6px 6px 0 0;
            -moz-border-radius: 6px 6px 0 0;
            -webkit-border-radius: 6px 6px 0 0;
        }

        &:last-of-type{
            border-radius: 0 0 6px 6px;
            -moz-border-radius: 0 0 6px 6px;
            -webkit-border-radius: 0 0 6px 6px;
        }
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