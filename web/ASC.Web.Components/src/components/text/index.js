import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import commonTextStyles from './common-text-styles';

const styleCss = css`
  outline: 0 !important;
  ${props => (typeof (props.fontSize) === 'string' && css`font-size: ${props.fontSize};`) ||
    (typeof (props.fontSize) === 'number' && css`font-size: ${props.fontSize}px;`)};
  font-weight: ${props => props.fontWeight
    ? props.fontWeight
    : props.isBold == true ? 700 : 500};
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

const Text = ({ title, tag, as, ...rest }) => {
  //console.log("Text render", rest)
  return (
    <StyledText as={!as && tag ? tag : as} title={title} {...rest} />
  );
};

Text.propTypes = {
  as: PropTypes.string,
  tag: PropTypes.string,
  title: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  fontWeight: PropTypes.number,
  backgroundColor: PropTypes.string,
  truncate: PropTypes.bool,
  isBold: PropTypes.bool,
  isInline: PropTypes.bool,
  isItalic: PropTypes.bool,
  display: PropTypes.string
};

Text.defaultProps = {
  title: '',
  color: '#333333',
  fontSize: 13,
  truncate: false,
  isBold: false,
  isInline: false,
  isItalic: false,
};

export default Text;