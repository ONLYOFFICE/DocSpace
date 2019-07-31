import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import Avatar from '../../components/avatar'
import IconButton from '../icon-button';

const itemTruncate = css`
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
`;

const fontStyle = css`
    font-family: Open Sans;
    font-style: normal;
`;

const StyledDropdownItem = styled.button`
    width: ${props => (props.isSeparator ? 'calc(100% - 32px)' : '100%')};
    height: ${props => (props.isSeparator && '1px')};
    line-height: ${props => (props.isSeparator ? '1px' : '36px')};

    margin: ${props => (props.isSeparator ? '0 16px' : '0')};
    padding: ${props => (props.isUserPreview ? '0px' : '0 16px')};

    border: ${props => (props.isSeparator ? '0.5px solid #ECEEF1' : '0')};
    cursor: ${props => ((!props.isSeparator || !props.isUserPreview) ? 'pointer' : 'default')};

    display: ${props => props.isUserPreview ? 'inline-block' : 'block'};
    
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

const UserPreview = styled.div`
    position: relative;
    height: 76px;
    background: linear-gradient(200.71deg, #2274AA 0%, #0F4071 100%);
    border-radius: 6px 6px 0px 0px;
    padding: 15px;
    cursor: default;
`;

const AvatarWrapper = styled.div`
    & > div > div {
        bottom: 14px;
    }

    display: inline-block;
    float: left;
`;

const UserNameWrapper = styled.div`
    ${fontStyle}

    font-size: 16px;
    line-height: 28px;
    color: #FFFFFF;
    margin-left: 60px;
    max-width:300px;
    ${itemTruncate}
`;

const UserEmailWrapper = styled.div`
    ${fontStyle}

    font-weight: normal;
    font-size: 11px;
    line-height: 16px;
    color: #FFFFFF;
    margin-left: 60px;
    max-width:300px;
    ${itemTruncate}
`;

const IconWrapper = styled.span`
    display: inline-block;
    margin-right: 8px;
`;

const DropDownItem = React.memo(props => {
  //console.log("DropDownItem render");
  const { isSeparator, isUserPreview, label, icon } = props;
  return (
    <StyledDropdownItem {...props} >
      {icon &&
        <IconWrapper>
          <IconButton size={16} iconName={icon} color={props.disabled || props.isHeader ? '#A3A9AE' : '#333333'} />
        </IconWrapper>
      }
      {isSeparator ? '\u00A0' : !isUserPreview && label}
      {isUserPreview &&
        <UserPreview {...props}>
          <AvatarWrapper>
            <Avatar size='medium'
              role={props.role}
              source={props.source}
              userName={props.userName}
            />
          </AvatarWrapper>
          <UserNameWrapper>{props.userName}</UserNameWrapper>
          <UserEmailWrapper>{label}</UserEmailWrapper>
        </UserPreview>
      }
    </StyledDropdownItem>
  );
});

DropDownItem.propTypes = {
  isSeparator: PropTypes.bool,
  isUserPreview: PropTypes.bool,
  isHeader: PropTypes.bool,
  tabIndex: PropTypes.number,
  label: PropTypes.string,
  disabled: PropTypes.bool,
  icon: PropTypes.string
};

DropDownItem.defaultProps = {
  isSeparator: false,
  isUserPreview: false,
  isHeader: false,
  tabIndex: -1,
  label: '',
  disabled: false
};

export default DropDownItem