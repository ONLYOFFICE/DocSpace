import React, { useState, useEffect, useRef } from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import GroupButton from '../group-button'

const StyledGroupButtonsMenu = styled.div`
    position: sticky;
    top: 0;
    background: #FFFFFF;
    box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.100306);
    height: 56px;
    list-style: none;
    padding: 0 18px 19px 0;

    .hidden {
        display: none;
    }
`;

const CloseButton = styled.div`
    position: absolute;
    right: 20px;
    top: 20px;
    width: 20px;
    height: 20px;

    &:hover{
        cursor: pointer;
    }

    &:before, &:after {
        position: absolute;
        left: 15px;
        content: ' ';
        height: 20px;
        width: 1px;
        background-color: #D8D8D8;
    }

    &:before {
        transform: rotate(45deg);
    }

    &:after {
        transform: rotate(-45deg);
    }
`;

const padding = 4;
var lastWidth = 0;

const collapseButtons = (menu) => {
    if (menu == undefined) return;
    
    var groupMenu = menu,
        groupMenuWidth = groupMenu.clientWidth,
        groupButtons = groupMenu.querySelectorAll('div[class*="-0"]:not(.more):not(.hidden)'),
        groupButtonsMore = groupMenu.querySelector('div.more[class*="-0"]'),
        groupButtonsMoreWidth = groupButtonsMore.clientWidth || 0,
        groupButtonsWidthArray = getButtonsWidthArray(groupButtons),
        groupButtonsWidth = getButtonsWidth(groupButtonsWidthArray),
        lastHidden = getLastHidden(groupMenu),
        lastHiddenWidth = (lastHidden != undefined) ? lastHidden.clientWidth : 0,
        lastGroupButton = groupButtons[groupButtons.length -1],
        moreThanMenu = groupButtonsWidth + groupButtonsMoreWidth - lastHiddenWidth,
        lessThanMenu = groupButtonsWidth + groupButtonsMoreWidth + lastHiddenWidth + lastGroupButton.clientWidth * 1.3;

    if (lastWidth !== 0 && lastWidth > groupMenuWidth){
        if (moreThanMenu > groupMenuWidth) {
            lastGroupButton.classList.add('hidden');
        }
    } else {
        if (lessThanMenu < groupMenuWidth && lastHidden != undefined) {
            lastHidden.classList.remove('hidden');
        }
    }

    if (lastHidden == undefined){
        groupButtonsMore.classList.add('hidden');
    } else {
        groupButtonsMore.classList.remove('hidden');
    }

    lastWidth = groupMenuWidth;
}

const getButtonsWidthArray = (buttons) => {
    return Array.prototype.slice.call(buttons).map((button => button.clientWidth + padding * 2));
}

const getButtonsWidth = (buttonsWidthArray) => {
    return buttonsWidthArray.reduce((a,b) => a + b);
}

const getLastHidden = (buttons) => {
    return buttons.querySelectorAll('div[class*="-0"].hidden:not(.more)')[0];
}

const GroupButtonsMenu = props => {
    const { children, needCollapse } = props;
    const ref = useRef(null);

    const collapseEvent = (e) => (React.Children.toArray(props.children).length && needCollapse)
        ? collapseButtons(ref.current) 
        : e.preventDefault();

    useEffect(() => {
        window.addEventListener('load', collapseEvent(event));
        window.onresize = (e) => collapseEvent(e);
    });

    return (
        <StyledGroupButtonsMenu ref={ref} {...props}>
            {children}
            {needCollapse && <GroupButton className="more" isDropdown label='More'>{children}</GroupButton>}
            <CloseButton/>
        </StyledGroupButtonsMenu>
    );
}

GroupButtonsMenu.propTypes = {
    needCollapse: PropTypes.bool
}

GroupButtonsMenu.defaultProps = {
    needCollapse: false
}

export default GroupButtonsMenu;