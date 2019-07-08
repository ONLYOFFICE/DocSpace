import React, {useState} from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Icons } from '../icons'

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

const colorCss = css`
    color: ${props =>
        (props.color === 'black' && '#333333') ||
        (props.color === 'gray' && '#A3A9AE') ||
        (props.color === 'blue' && '#316DAA')
    };
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

const Caret = styled(Icons.ExpanderDownIcon)`
    width: 10px;
    margin-left: 5px;
    margin-top: -4px;
`;

const StyledLink = styled(SimpleLink).attrs((props) => ({
    href: props.href,
    target: props.target,
    rel: props.target === '_blank' && 'noopener noreferrer',
    title: props.title
}))`
    ${colorCss};
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
        
${props => (props.isHovered && 
    css`
        ${hoveredCss}
    `)
}

${props => (props.type === 'action' && (props.isHovered || props.dropdownType === 'alwaysDotted') && 
    css`
        ${dottedCss}
    `)
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

${props => (props.isSemitransparent
 && 
    css`
        opacity: 0.5;
    `)
}

`;

const Link = props => {
    const [isHovered, toggle] = useState(false);

    return (
        <span
        onMouseEnter={() => {props.dropdownType === 'appearDottedAfterHover' && toggle(!isHovered)}}
        onMouseLeave={() => {props.dropdownType === 'appearDottedAfterHover' && toggle(!isHovered)}}>
        <StyledLink {...props} /> 
        {(props.dropdownType === 'alwaysDotted' || (isHovered && props.dropdownType === 'appearDottedAfterHover')) &&  <Caret size='small' isfill={true} color={getDropdownColor(props.color)} /> }
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