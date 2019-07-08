import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import Avatar from '../../components/avatar'

const StyledDropdownItem = styled.button`
    width: ${props => (props.isSeparator ? 'calc(100% - 32px)' : '100%')};
    text-align: left;
    background: none;
    border: ${props => (props.isSeparator ? '0.5px solid #ECEEF1' : '0')};
    color: #333333;
    height: ${props => (props.isSeparator && '1px')};
    cursor: ${props => ((!props.isSeparator || !props.isUserPreview) ? 'pointer' : 'default')};
    box-sizing: border-box;
    line-height: ${props => (props.isSeparator ? '1px' : '36px')};
    margin: ${props => (props.isSeparator ? '0 16px' : '0')};
    padding: ${props => (props.isUserPreview ? '0px' : '0 16px')};
    text-decoration: none;
    display: block;

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

const UserPreview = styled.div`
    position: relative;
    min-width: 200px;
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
`;

const UserNameWrapper = styled.div`
    position: absolute;
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 16px;
    line-height: 22px;
    color: #FFFFFF;

    top: 18px;
    left: 73px;
`;

const UserEmailWrapper = styled.div`
    position: absolute;
    font-family: Open Sans;
    font-style: normal;
    font-weight: normal;
    font-size: 11px;
    line-height: 15px;
    color: #FFFFFF;

    top: 40px;
    left: 73px;
`;

const DropDownItem = props => {
    const {isSeparator, isUserPreview, tabIndex, onClick, label} = props;
    return (
        <StyledDropdownItem {...props} >
            {isSeparator ? '\u00A0' : !isUserPreview  && label}
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
};

DropDownItem.propTypes = {
    isSeparator: PropTypes.bool,
    isUserPreview: PropTypes.bool,
    tabIndex: PropTypes.number,
    onClick: PropTypes.func,
    label: PropTypes.string
};

DropDownItem.defaultProps = {
    isSeparator: false,
    isUserPreview: false,
    tabIndex: -1,
    onClick: (e) => console.log('Button "' + e.target.innerText + '" clicked!'),
    label: ''
};

export default DropDownItem