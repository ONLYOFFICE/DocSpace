import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';

const arrowDropdown = css`
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: ${props => 
        ((props.dropdownColor === 'filter' || props.dropdownColor === 'sorting') && '4px solid #888') || 
        (props.dropdownColor === 'profile' && '4px solid #cbcbcc') || 
        (props.dropdownColor === 'number' && '4px solid #212121') || 
        ((props.dropdownColor === 'email' || props.dropdownColor === 'group') && '4px solid #666')
    };

    content: "";
    height: 0;
    position: absolute;
    right: ${props => 
        (props.dropdownRightIndent)
    };

    top: 50%;
    width: 0;
    margin-top: -2px;
`;

const textOverflowCss = css`
    text-overflow: ellipsis;
    -o-text-overflow: ellipsis;
    -moz-text-overflow: ellipsis;
    -webkit-text-overflow: ellipsis;
`;

const boldCss = css`
    font-weight: bold;
`;

const colorCss = css`
    color: ${props =>
        (props.color === 'black' && '#333333') ||
        (props.color === 'gray' && '#A3A9AE') ||
        (props.color === 'blue' && '#316DAA') ||
        (props.color === 'filter' && '#1a6db3') ||
        (props.color === 'profile' && '#C5C5C5')
    };
`;

const hoveredCss = css`
    ${colorCss};
    border-bottom: ${props => (props.isHoverDotted && props.type === 'action' ?  '1px dotted;' : 'none')};
    text-decoration: ${props => (props.type === 'page' ? 'underline' : 'none')};
`;

const visitedCss = css`
    ${colorCss};
`;


const fontSizeCss = css`
    font-size: ${props => props.fontSize};
`;

const dottedCss = css`
    border-bottom: 1px dotted;
`;

const StyledLink = styled.a.attrs((props) => ({
    href: props.href,
    target: props.target,
    rel: props.target === '_blank' ? 'noopener noreferrer' : props.rel ? props.rel : undefined,
    title: props.title
}))`
    ${colorCss};
    ${fontSizeCss};
    cursor: pointer;
    position: relative;
    text-decoration: none;
    font-weight: ${props => (props.isBold && css `${boldCss}`)};

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

${props => (props.type === 'action' && props.isDotted && (!props.isHovered || props.isHoverDotted) && 
    css`
        ${dottedCss}
    `)
}
        
${props => (props.type === 'action' && props.isDropdown && !props.displayDropdownAfterHover &&  
    css`
        &:after {
            ${arrowDropdown}
        }`)
}

${props => (props.isTextOverflow && 
    css`
        ${textOverflowCss}
    `)
}

${props => (props.isBold && 
    css`
        ${boldCss}
    `)
} 

${props => (props.type === 'action' && props.isDropdown && props.displayDropdownAfterHover && 
    css`
        &:hover { 
            :after {
                ${arrowDropdown}
                }
        }
    `)
}
`;

const Link = props => <StyledLink {...props} />;

Link.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue', 'filter', 'profile']),
    displayDropdownAfterHover: PropTypes.bool,
    dropdownColor: PropTypes.oneOf(['filter', 'profile', 'sorting','number','email',  'group']),
    dropdownRightIndent: PropTypes.string,
    dropdownType: PropTypes.oneOf(['filter', 'menu', 'none']),
    fontSize: PropTypes.string,
    href: PropTypes.string,
    isBold: PropTypes.bool,
    isDropdown: PropTypes.bool,
    isDotted: PropTypes.bool,
    isHoverDotted: PropTypes.bool,
    isHovered: PropTypes.bool,
    isTextOverflow: PropTypes.bool,
    onClick: PropTypes.func,
    rel: PropTypes.string,
    target: PropTypes.oneOf(['_blank', '_self', '_parent', '_top']),
    title: PropTypes.string,
    type: PropTypes.oneOf(['action', 'page'])  
};

Link.defaultProps = {
    color: 'black',
    dropdownRightIndent: '-10px',
    fontSize: '12px',
    href: undefined,
    isBold: false,
    isHovered: false,
    isTextOverflow: true,
    type: 'page'
}

export default Link;