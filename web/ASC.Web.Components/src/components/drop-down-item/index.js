import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

const StyledDropdownItem = styled.button`
    width: ${props => (props.isSeparator ? 'calc(100% - 32px)' : '100%')};
    text-align: left;
    background: none;
    border: ${props => (props.isSeparator ? '0.5px solid #ECEEF1' : '0')};
    color: #333333;
    height: ${props => (props.isSeparator && '1px')};
    cursor: ${props => (props.isSeparator ? 'pointer' : 'default')};
    box-sizing: border-box;
    line-height: ${props => (props.isSeparator ? '1px' : '36px')};
    margin: ${props => (props.isSeparator ? '0 16px' : '0')};
    padding: 0 16px;
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
        ${props => (!props.isSeparator 
            ? `
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
            }`
            : `
            cursor: default;
            `
        )}
    }
`;

const DropDownItem = props => {
    const {isSeparator, tabIndex, onClick, label} = props;
    return (
        <StyledDropdownItem {...props} >
            {isSeparator ? '\u00A0' : label}
        </StyledDropdownItem>
    );
};

DropDownItem.propTypes = {
    isSeparator: PropTypes.bool,
    tabIndex: PropTypes.number,
    onClick: PropTypes.func,
    label: PropTypes.string
};

DropDownItem.defaultProps = {
    isSeparator: false,
    tabIndex: -1,
    onClick: (e) => console.log('Button "' + e.target.innerText + '" clicked!'),
    label: 'Dropdown item'
};

export default DropDownItem