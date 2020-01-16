import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import commonTextStyles from './common-text-styles';

const styleCss = css`
  font-size: ${props => props.fontSizeProp};  
  outline: 0 !important;
  font-weight: ${props => props.fontWeightProp
    ? props.fontWeightProp
    : props.isBold == true ? 700 : 'normal'};
  ${props => props.isItalic == true && css`font-style: italic;`}
  ${props => props.backgroundColor && css`background-color: ${props => props.backgroundColor};`}
  ${props => props.isInline
    ? css`display: inline-block;`
    : props.display && css`display: ${props => props.display};`}
  margin: 0;
`;

const StyledText = styled.p`
  ${styleCss};
  ${commonTextStyles};
`;

const Text = ({ title, tag, as, fontSize, fontWeight, color, ...rest }) => {
  //console.log("Text render", rest)
  return (
    <StyledText
      fontSizeProp={fontSize}
      fontWeightProp={fontWeight}
      colorProp={color}
      as={!as && tag ? tag : as}
      title={title}
      {...rest}
    />
  );
};

Text.propTypes = {
  as: PropTypes.string,
  tag: PropTypes.string,
  title: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  backgroundColor: PropTypes.string,
  truncate: PropTypes.bool,
  isBold: PropTypes.bool,
  isInline: PropTypes.bool,
  isItalic: PropTypes.bool,
  display: PropTypes.string
};

Text.defaultProps = {
  title: null,
  color: '#333333',
  fontSize: '13px',
  truncate: false,
  isBold: false,
  isInline: false,
  isItalic: false,
};

export default Text;