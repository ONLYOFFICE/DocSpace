import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import IconButton from '../icon-button';

const itemTruncate = css`
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
`;

const fontStyle = css`
    font-family: 'Open Sans',sans-serif,Arial;
    font-style: normal;
`;

const StyledDropdownItem = styled.button`
    width: ${props => (props.isSeparator ? 'calc(100% - 32px)' : '100%')};
    height: ${props => (props.isSeparator && '1px')};
    line-height: ${props => (props.isSeparator ? '1px' : '36px')};

    margin: ${props => (props.isSeparator ? '0 16px' : '0')};
    padding: 0 16px;

    border: ${props => (props.isSeparator ? '0.5px solid #ECEEF1' : '0')};
    cursor: ${props => (!props.isSeparator ? 'pointer' : 'default')};

    display: block;
    
    color: ${props => props.disabled || props.isHeader ? '#A3A9AE' : '#333333'};
    text-transform: ${props => props.isHeader ? 'uppercase' : 'none'};

    box-sizing: border-box;
    text-align: left;
    background: none;
    text-decoration: none;
    
    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;
    stroke: none;

    ${fontStyle}

    font-weight: 600;
    font-size: 13px;

    ${itemTruncate}
    
    &:hover{
        ${props => props.isSeparator || props.disabled || props.isHeader
    ? `cursor: default;`
    : `
            background-color: #F8F9F9;
            width: 100%;
            text-align: left;

            &:first-of-type {
                border-radius: 6px 6px 0 0;
                -moz-border-radius: 6px 6px 0 0;
                -webkit-border-radius: 6px 6px 0 0;
            }

            &:last-of-type {
                border-radius: 0 0 6px 6px;
                -moz-border-radius: 0 0 6px 6px;
                -webkit-border-radius: 0 0 6px 6px;
            }`
  }
    }
`;

const IconWrapper = styled.span`
    display: inline-block;
    margin-right: 8px;
`;

const DropDownItem = React.memo(props => {
  //console.log("DropDownItem render");
  const { isSeparator, label, icon } = props;
  const color = props.disabled || props.isHeader ? '#A3A9AE' : '#333333';
  
  return (
    <StyledDropdownItem {...props} >
      {icon &&
        <IconWrapper>
          <IconButton size={16} iconName={icon} color={color} />
        </IconWrapper>
      }
      {isSeparator ? '\u00A0' : label}
    </StyledDropdownItem>
  );
});

DropDownItem.propTypes = {
  isSeparator: PropTypes.bool,
  isHeader: PropTypes.bool,
  tabIndex: PropTypes.number,
  label: PropTypes.string,
  disabled: PropTypes.bool,
  icon: PropTypes.string
};

DropDownItem.defaultProps = {
  isSeparator: false,
  isHeader: false,
  tabIndex: -1,
  label: '',
  disabled: false
};

export default DropDownItem