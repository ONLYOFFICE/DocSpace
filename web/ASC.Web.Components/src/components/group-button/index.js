import React, { useState, useEffect, useRef } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import { Icons } from '../icons'
import DropDown from '../drop-down'
import Checkbox from '../checkbox'

const textColor = '#333333',
    disabledTextColor = '#A3A9AE';

const activatedCss = css`
    cursor: pointer;
`;

const hoveredCss = css`
    cursor: pointer;
`;

const StyledGroupButton = styled.div`
    position: relative;
    display: inline-flex;
    vertical-align: middle;
`;

const StyledDropdownToggle = styled.div`
    font-family: Open Sans;
    font-style: normal;
    font-weight: ${props => props.fontWeight};
    font-size: 14px;
    line-height: 19px;

    cursor: default;

    color: ${props => (props.disabled ? disabledTextColor : textColor)};
    
    float: left;
    height: 19px;
    margin: 18px 12px 19px 12px;
    overflow: hidden;
    padding: 0px;

    text-align: center;
    text-decoration: none;
    white-space: nowrap;

    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;

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
`;

const Caret = styled(Icons.ExpanderDownIcon)`
    width: 10px;
    margin-left: 4px;
`;

const Separator = styled.div`
    vertical-align: middle;
    border: 0.5px solid #ECEEF1;
    width: 1px;
    height: 24px;
    margin-top: 16px;
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
    const { label, isDropdown, opened, disabled, isSeparator} = props;
    const [isOpen, toggle] = useState(opened);

    let ref = useRef(null);

    useOuterClickNotifier(() => toggle(false), ref);

    const dropMenu = <DropDown isOpen={isOpen} {...props}/>;

    const dropDownButton =
        <StyledDropdownToggle {...props} onClick={() => { disabled ? false : toggle(!isOpen) }}>
            {label}
            <Caret size='small' color={disabled ? disabledTextColor : textColor} />
        </StyledDropdownToggle>;

    return (
        <StyledGroupButton ref={ref}>
            {isDropdown
                ? {...dropDownButton}
                : <StyledDropdownToggle {...props} >
                    {label}
                </StyledDropdownToggle>
            }
            {isSeparator && <Separator/>}
            {isDropdown && {...dropMenu}}
        </StyledGroupButton>
    );
}

GroupButton.propTypes = {
    label: PropTypes.string,
    disabled: PropTypes.bool,
    activated: PropTypes.bool,
    opened: PropTypes.bool,
    hovered: PropTypes.bool,
    isDropdown: PropTypes.bool,
    isSeparator: PropTypes.bool,
    tabIndex: PropTypes.number,
    onClick: PropTypes.func,
    fontWeight: PropTypes.string
};

GroupButton.defaultProps = {
    label: 'Group button',
    disabled: false,
    activated: false,
    opened: false,
    hovered: false,
    isDropdown: false,
    isSeparator: false,
    tabIndex: -1,
    fontWeight: '600'
};

export default GroupButton