import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';

const fontSize = css`
    ${props =>
        (props.elementType === 'h1' && 23) ||
        (props.elementType === 'h2' && 19) ||
        (props.elementType === 'h3' && 15) ||
        (props.elementType === 'p'  && 13)
    }
`;

const fontWeight = css`
    ${props =>
        (props.elementType === 'h1' && 600) ||
        (props.elementType === 'h2' && 600) ||
        (props.elementType === 'h3' && 600) ||
        (props.elementType === 'p'  && 500)
    }
`;

const fontColor = css`
    ${props =>
        (props.styleType === 'default' && '#333333') ||
        (props.styleType === 'grayBackground' && '#657077') ||
        (props.styleType === 'metaInfo' && '#A3A9AE') ||
        (props.styleType === 'disabled'  && '#ECEEF1')
    }
`;

const StyledText = styled.p`
    font-family: 'Open Sans',sans-serif,Arial;
    font-size: ${fontSize}px;
    font-weight: ${fontWeight};
    color: ${fontColor};
    background-color:  ${props => (props.styleType === 'grayBackground' && '#F8F9F9')};
    text-align: left;
    max-width: 1000px;
    ${props => (props.truncate === true && 'white-space: nowrap; overflow: hidden; text-overflow: ellipsis;' )}
`;

const Text = props => <StyledText {...props} title={props.title}>
    {props.text}
</StyledText>;

Text.propTypes = {
    fontSize: PropTypes.number,
    elementType: PropTypes.string,
    styleType: PropTypes.string,
    title: PropTypes.string,
    truncate: PropTypes.bool,
    text: PropTypes.string
};

Text.defaultProps = {
    elementType: 'p',
    styleType: 'default',
    title: '',
    truncate: false,
    text: ''
};

export default Text;