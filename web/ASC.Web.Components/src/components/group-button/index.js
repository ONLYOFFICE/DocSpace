import React, { useState, useEffect, useRef } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import { Icons } from '../icons'
import DropDown from '../drop-down'

const backgroundColor = '#ebebeb',
    disabledBackgroundColor = '#f7f7f7',
    activatedBackgroundColor = '#e2e2e2',
    hoveredBackgroundColor = '#f5f5f5',
    primaryBackgroundColor = '#2da7db',
    primaryDisabledBackgroundColor = '#a6dcf2',
    primaryActivatedBackgroundColor = '#1f97ca',
    primaryHoveredBackgroundColor = '#3db8ec';

const borderColor = '#c4c4c4',
    disabledBorderColor = '#ebebeb',
    activatedBorderColor = '#dadada',
    hoveredBorderColor = '#d4d4d4',
    primaryBorderColor = 'transparent';

const textColor = '#666666',
    disabledTextColor = '#999999',
    primaryTextColor = '#ffffff';

const activatedCss = css`
    cursor: pointer;
    border: 1px solid ${props => props.primary ? primaryBorderColor : activatedBorderColor} !important;
    background-color: ${props => props.primary ? primaryActivatedBackgroundColor : activatedBackgroundColor} !important;
`;

const hoveredCss = css`
    cursor: pointer;
    border: 1px solid ${props => props.primary ? primaryBorderColor : hoveredBorderColor};
    background-color: ${props => props.primary ? primaryHoveredBackgroundColor : hoveredBackgroundColor};
`;

const StyledGroupButton = styled.div`
    position: relative;
    display: inline-flex;
    vertical-align: middle;
    box-sizing: border-box;
`;

const StyledDropdownToggle = styled.button`
    font: normal 12px 'Open Sans', sans-serif;
    cursor: default;

    color: ${props => (props.primary && primaryTextColor) || (props.disabled ? disabledTextColor : textColor)};
    background-color: ${props => (props.disabled 
        ? (props.primary ? primaryDisabledBackgroundColor : disabledBackgroundColor) 
        : (props.primary ? primaryBackgroundColor : backgroundColor))};
    border: 1px solid ${props => (props.disabled 
        ? (props.primary ? primaryBorderColor : disabledBorderColor) 
        : (props.primary ? primaryBorderColor : borderColor))};
    
    float: left;
    height: 21px;
    margin-right: ${props => props.splitted ? '0px' : '4px'};
    margin-left: ${props => props.splitted ? '-1px' : '0px'};
    overflow: hidden;
    padding: 1px 5px 1px;
    text-align: center;
    text-decoration: none;
    white-space: nowrap;
    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;

    border-radius: ${props => props.splitted ? '0px 3px 3px 0px' : '3px'};
    -moz-border-radius: ${props => props.splitted ? '0px 3px 3px 0px' : '3px'};
    -webkit-border-radius: ${props => props.splitted ? '0px 3px 3px 0px' : '3px'};

    ${props => !props.disabled && (props.activated ? `${activatedCss}` : css`
        &:active {
            ${activatedCss}
        }`)
    }

    ${props => !props.disabled && (props.hovered ? `${hoveredCss}` : css`
        &:hover {
            ${hoveredCss}
        }`)
    }

    ${props => props.isCheckbox & !props.splitted && css`
        padding: 3px 5px 1px;
    `}
`;

const StyledButton = styled(StyledDropdownToggle)`
    margin-right: 0px;
    margin-left: 0px;

    border-radius: ${props => props.splitted ? '3px 0px 0px 3px' : '3px'};
    -moz-border-radius: ${props => props.splitted ? '3px 0px 0px 3px' : '3px'};
    -webkit-border-radius: ${props => props.splitted ? '3px 0px 0px 3px' : '3px'};

    ${props => props.isCheckbox & props.splitted && css`
        padding: 3px 5px 1px;
    `}
`;

const Caret = styled(Icons.ExpanderDownIcon)`
    width: 10px;
    margin-left: 4px;

    ${props => props.isCheckbox && (props.splitted
        ? css`
            margin-top: -4px;
            margin-left: 0;
            ` 
        : css`
            margin-top: -6px;
            `)
    }

    ${props => !props.isCheckbox && (props.splitted 
        ? css`
            margin-top: -2px;
            margin-left: 0;
            `
        : css`
            margin-top: -3px;
            `)
    }
`;

const useOuterClickNotifier = (onOuterClick, ref) => {
    useEffect(() => { 
        const handleClick = (e) => !ref.current.contains(e.target) && onOuterClick(e);

        if (ref.current) {
            document.addEventListener("click", handleClick);
        }

        return () => document.removeEventListener("click", handleClick);
    },
    [onOuterClick, ref]
    );
}

const GroupButton = (props) => {
    const { text, splitted, isDropdown, isCheckbox, opened, disabled, primary, clickAction } = props;
    const [isOpen, toggle] = useState(opened);
    const ref = useRef(null);

    useOuterClickNotifier((e) => toggle(false), ref);

    const dropMenu = <DropDown isOpen={isOpen} {...props}/>;

    const splittedDropButton =
        <>
            <StyledButton {...props} onClick={clickAction}>
                {text}
                {isCheckbox && <input type='checkbox' disabled={disabled ? 'disabled' : ''} />}
            </StyledButton>
            <StyledDropdownToggle {...props} onClick={() => { disabled ? false : toggle(!isOpen) }}>
                <Caret 
                    splitted={splitted} 
                    isCheckbox={isCheckbox} 
                    size='small' 
                    color={primary && primaryTextColor || (disabled ? disabledTextColor : textColor)} 
                />
            </StyledDropdownToggle>
        </>;

    const singleDropButton =
        <StyledDropdownToggle {...props} onClick={() => { disabled ? false : toggle(!isOpen) }}>
            {text}
            {isCheckbox && <input type='checkbox' disabled={disabled ? 'disabled' : ''} />}
            <Caret 
                splitted={splitted} 
                isCheckbox={isCheckbox}
                size='small'
                color={primary && primaryTextColor || (disabled ? disabledTextColor : textColor)} 
            />
        </StyledDropdownToggle>;

    return (
        <StyledGroupButton ref={ref} {...props}>
            {isDropdown
                ? (splitted
                    ? { ...splittedDropButton }
                    : { ...singleDropButton })
                : <StyledDropdownToggle {...props} onClick={clickAction}>
                    {text}
                    {isCheckbox && <input type='checkbox' disabled={disabled ? 'disabled' : ''} />}
                </StyledDropdownToggle>
            }
            {isDropdown && { ...dropMenu }}
        </StyledGroupButton>
    );
}

GroupButton.propTypes = {
    text: PropTypes.string,
    primary: PropTypes.bool,
    disabled: PropTypes.bool,
    activated: PropTypes.bool,
    opened: PropTypes.bool,
    hovered: PropTypes.bool,
    splitted: PropTypes.bool,
    isCheckbox: PropTypes.bool,
    isDropdown: PropTypes.bool,
    tabIndex: PropTypes.number,
    clickAction: PropTypes.func
};

GroupButton.defaultProps = {
    text: 'Default text',
    primary: false,
    disabled: false,
    activated: false,
    opened: false,
    hovered: false,
    splitted: false,
    isCheckbox: false,
    isDropdown: false,
    tabIndex: -1,
    clickAction: (e) => console.log('Button "' + e.target.innerText + '" clicked!')
};

export default GroupButton