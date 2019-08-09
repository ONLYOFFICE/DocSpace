import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Text } from '../text';

const SimpleLink = ({ rel, isBold, fontSize, isTextOverflow,
    isHovered, isSemitransparent, type, color, title, onClick,
    ...props }) => <a {...props}></a>;

const opacityCss = css`
    opacity: ${props =>
        (props.isSemitransparent && '0.5')};
`;

const getColor = color => {
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
    color: ${props => getColor(props.color)};
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

const Link = props => {

    const { isBold, title, fontSize, color } = props;

    const onClick = (e) => {
        !props.href && e.preventDefault();
        props.onClick && props.onClick(e);
    }

    console.log("Link render");

    return (
        <StyledLink {...props}>
            <Text.Body
                as="span"
                color={getColor(color)}
                fontSize={fontSize}
                onClick={onClick}
                isBold={isBold}
                title={title}
            >
                {props.children}
            </Text.Body>
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
    fontSize: 13,
    href: undefined,
    isBold: false,
    isHovered: false,
    isSemitransparent: false,
    isTextOverflow: true,
    type: 'page'
}

export default Link;
