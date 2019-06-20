import React, { useState, useEffect, useRef } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import DropDown from '../drop-down';
import { Icons } from '../icons'


const backgroundColor = '#ED7309',
    disableBackgroundColor = '#FFCCA6',
    hoverBackgroundColor = '#FF8932',
    clickBackgroundColor = '#C96C27';

const hoveredCss = css`
    background-color: ${hoverBackgroundColor};
    cursor: pointer;
`;

const clickCss = css`
    background-color: ${clickBackgroundColor};
    cursor: pointer;
`;

const arrowDropdown = css`
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: 4px solid white;
    content: "";
    height: 0;
    margin-top: -1px;
    position: absolute;
    right: 8px;
    top: 50%;
    width: 0;
`;

const notDisableStyles = css`
    &:hover {
        ${hoveredCss}
    };

    &:active {
        ${clickCss}
    };
`;

const notDropdown = css`
    &:after {
        display:none;
    }

    border-top-right-radius: 0;
    border-bottom-right-radius:0;
    
`;

const GroupMainButton = styled.div`
    position: relative;
    display: grid;
    Grid-template-columns: ${props => (props.isDropdown ? "1fr" : "1fr 32px")};
    ${props => (!props.isDropdown && "grid-column-gap: 1px")};
`;

const StyledDropDown = styled(DropDown)`
    width:100%;
`;

const StyledMainButton = styled.div`
    position: relative;
    display: block;
    vertical-align: middle;
    box-sizing: border-box;
    background-color: ${props => props.isDisable ? disableBackgroundColor : backgroundColor};
    padding: 5px 11px;
    color: #ffffff;
    border-radius: 3px;
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    font-weight: bold;
    font-size: 16px;
    line-height: 22px;

    &:after {
        ${arrowDropdown}
    }

    ${props => (!props.isDisable && notDisableStyles)}
    ${props => (!props.isDropdown && notDropdown)}

    & > svg {
        display: block;
        margin: auto;
        height: 100%;
    } 
`;

const StyledSecondaryButton = styled(StyledMainButton)`
    display: inline-block;
    height: 32px;
    padding:0;
    border-radius: 3px;
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    border-top-left-radius: 0;
    border-bottom-left-radius:0;
`;

const useOuterClickNotifier = (onOuterClick, ref) => {
    useEffect(() => { 
        const handleClick = (e) => !ref.current.contains(e.target) && onOuterClick(e);

        if (ref.current) {
            console.log("thisentry")
            document.addEventListener("click", handleClick);
        }

        return () => document.removeEventListener("click", handleClick);
    },
    [onOuterClick, ref]
    );
}


const MainButton = (props) => {
    const { text, isDropdown, opened } = props;
    const [isOpen, toggle] = useState(opened);
    const dropMenu = <StyledDropDown isOpen={isOpen} {...props}/>;
    const ref = useRef(null);
  
    useOuterClickNotifier((e) => toggle(false), ref);

    function stopAction(e) {
        e.preventDefault();
    }

    const SecondaryButtonIcon = moduleName => {
        
        switch (moduleName) {
          case "people":
            return <Icons.PeopleIcon size="medium" color='#ffffff' />;
          case "mail":
            return <Icons.RotateIcon size="medium" color='#ffffff' />;
          case "documents":
            return <Icons.UploadIcon size="medium" color='#ffffff' />;
          default:
            return (
                <div></div>
            );
        }
      };

    return(
        <GroupMainButton {...props} ref={ref}>
            <StyledMainButton {...props} onClick={ 
                    !props.isDisable 
                        ? !props.isDropdown 
                            ? props.clickAction 
                            : () => { toggle(!isOpen) }
                        : stopAction}>
                {text}
            </StyledMainButton>
            {isDropdown 
                ?   { ...dropMenu } 
                
                :   <StyledSecondaryButton {...props} onClick={ !props.isDisable ? props.clickActionSecondary : stopAction}> 
                        {SecondaryButtonIcon(props.moduleName)}
                    </StyledSecondaryButton>}
        </GroupMainButton>
    )
}

MainButton.propTypes = {
    text: PropTypes.string,
    id: PropTypes.number,
    isDisable: PropTypes.bool,
    isDropdown: PropTypes.bool,
    clickAction: PropTypes.func,
    clickActionSecondary: PropTypes.func,
    moduleName: PropTypes.string,
};

MainButton.defaultProps = {
    text: "Button",
    isDisable: false,
    isDropdown: true,
    moduleName: "",
};

export default MainButton;