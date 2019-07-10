import React, {useState, useRef, useEffect} from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Icons } from '../icons'
import DropDown from '../drop-down'

const SimpleLink = ({ rel, isBold, fontSize, isTextOverflow, isHovered, isSemitransparent, type, color, text, target, dropdownType,  ...props }) => <a {...props}>{text}</a>;

const getDropdownColor = color => {
    switch (color) {
        case 'gray':
            return '#A3A9AE';
        case 'blue':
            return '#316DAA';
        default:
            return '#333333';
    }
}

const opacityCss = css `
    opacity: ${props =>
        (props.isSemitransparent  && '0.5')};
`;

const colorCss = css`
    color: ${props => getDropdownColor(props.color)};
`;

const hoveredCss = css`
    ${colorCss};
    border-bottom: ${props => (props.type === 'action' ?  '1px dotted;' : 'none')};
    text-decoration: ${props => (props.type === 'page' ? 'underline' : 'none')};
`;

const visitedCss = css`
    ${colorCss};
`;

const dottedCss = css`
    border-bottom: 1px dotted;
`;

const Caret = styled(Icons.ExpanderDownIcon).attrs((props) => ({
    isSemitransparent: props.isSemitransparent
}))`
    width: 10px;
    margin-left: 5px;
    margin-top: -4px;
    ${opacityCss};
`;

const StyledLink = styled(SimpleLink).attrs((props) => ({
    href: props.href,
    target: props.target,
    rel: props.target === '_blank' && 'noopener noreferrer',
    title: props.title
}))`
    ${colorCss};
    ${opacityCss};
    font-size: ${props => props.fontSize}px;
    cursor: pointer;
    position: relative;
    text-decoration: none;
    font-weight: ${props => (props.isBold && 'bold')};
    
    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;

        &:hover { 
            ${hoveredCss};
        }

        &:visited { 
           ${visitedCss};
        }

        &:not([href]):not([tabindex]) {
            ${colorCss};
            text-decoration: none;

            &:hover {
                ${hoveredCss};
            }
        }      
        
${props => (props.isHovered && hoveredCss)
}

${props => (props.type === 'action' && (props.isHovered || props.dropdownType === 'alwaysDotted') && dottedCss)
}

${props => (props.isTextOverflow && 
    css`
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        -o-text-overflow: ellipsis;
        -moz-text-overflow: ellipsis;
        -webkit-text-overflow: ellipsis;
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


const Link = props => {
    let isDropdown;
    const [isHovered, toggle] = useState(false);
    const [isOpen, toggleDropdown] = useState(0);
    const dropMenu = <DropDown isOpen={isOpen} {...props}>
                    {props.children}
                     </DropDown>;

    const ref = useRef(null);
    props.dropdownType != 'none' ? isDropdown = true : isDropdown = false;
    
    function stopAction(e) {
        if (props.href === ''){
            e.preventDefault();
        }
    }

    useOuterClickNotifier((e) => toggleDropdown(false), ref);
    
    return (

        <span ref={ref}
        onMouseEnter={() => {props.dropdownType === 'appearDottedAfterHover' && toggle(!isHovered)}}
        onMouseLeave={() => {props.dropdownType === 'appearDottedAfterHover' && toggle(!isHovered)}}>
           
        <StyledLink {...props}  onClick={ 
                 isDropdown ?
                     () => { toggleDropdown(!isOpen) }
                : stopAction}/> 
        {isDropdown && (isHovered || props.dropdownType === 'alwaysDotted') &&  <Caret isSemitransparent={props.isSemitransparent} size='small' isfill={true} color={getDropdownColor(props.color)} /> }
        {isDropdown &&  dropMenu }
        </span>
        
    );
}

Link.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue']),
    dropdownType: PropTypes.oneOf(['alwaysDotted', 'appearDottedAfterHover', 'none']),
    fontSize: PropTypes.number,
    href: PropTypes.string,
    isBold: PropTypes.bool,
    isHovered: PropTypes.bool,
    isSemitransparent: PropTypes.bool,
    isTextOverflow: PropTypes.bool,
    onClick: PropTypes.func,
    target: PropTypes.oneOf(['_blank', '_self', '_parent', '_top']),
    text: PropTypes.string,
    title: PropTypes.string,
    type: PropTypes.oneOf(['action', 'page'])  
};

Link.defaultProps = {
    color: 'black',
    dropdownType: 'none',
    fontSize: 12,
    href: undefined,
    isBold: false,
    isHovered: false,
    isSemitransparent: false,
    isTextOverflow: true,
    type: 'page'
}

export default Link;
