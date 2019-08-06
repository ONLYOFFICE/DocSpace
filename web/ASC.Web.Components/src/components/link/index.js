import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Text } from '../text';

const SimpleLink = ({ rel, isBold, fontSize, isTextOverflow,
    isHovered, isSemitransparent, type, color, title,
    ...props }) => <a {...props}></a>;

const opacityCss = css`
    opacity: ${props =>
        (props.isSemitransparent && '0.5')};
`;

const colorCss = css`
    color: ${props =>
        (props.color === 'black' && '#333333') ||
        (props.color === 'gray' && '#A3A9AE') ||
        (props.color === 'blue' && '#316DAA')
    };
`;

const hoveredCss = css`
    ${colorCss};
    border-bottom: ${props => (props.type === 'action' ? '1px dotted;' : 'none')};
    text-decoration: ${props => (props.type === 'page' ? 'underline' : 'none')};
`;

const visitedCss = css`
    ${colorCss};
`;

const dottedCss = css`
    border-bottom: 1px dotted;
`;

const StyledLink = styled(SimpleLink).attrs((props) => ({
    href: props.href,
    rel: props.target === '_blank' && 'noopener noreferrer',
}))`
    ${opacityCss};
    text-decoration: none;
    user-select: none;
    cursor: pointer;

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

${props => (props.isHovered && hoveredCss)}

${props => (props.type === 'action' && props.isHovered &&
        dottedCss)}

${props => (props.isTextOverflow && css`
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    `)}

`;

const SimpleText = ({ color, fontSize, ...props }) => <Text.Body as="span" {...props} />
const StyledText = styled(SimpleText)`
    ${colorCss};
    font-size: ${props => props.fontSize}px;
`;

const Link = props => {
    const onClick = (e) => {
        !props.href && e.preventDefault();
        props.onClick && props.onClick(e);
    }

    console.log("Link render");

    return (
        <StyledLink {...props}>
            <StyledText
                onClick={onClick}
                fontSize={props.fontSize}
                color={props.color}
                isBold={props.isBold}
                title={props.title}
            >
                {props.children}
            </StyledText>
        </StyledLink>
    );
}

Link.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue']),
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
    fontSize: 12,
    href: undefined,
    isBold: false,
    isHovered: false,
    isSemitransparent: false,
    isTextOverflow: true,
    type: 'page'
}

export default Link;
