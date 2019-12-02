import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

const styleCss = css`
   font-family: 'Open Sans', sans-serif, Arial;
   font-size: ${props => props.fontSize}px;
   font-weight: ${props => props.fontWeight
         ? props.fontWeight
         : props.isBold == true ? 700 : 500};
   ${props => props.isItalic == true && css`font-style: italic;`}
   color: ${props => props.color};
   ${props => props.backgroundColor && css`background-color: ${props => props.backgroundColor};`}
   ${props => props.isInline
         ? css`display: inline-block;`
         : props.display && css`display: ${props => props.display};`}
   text-align: left;
   ${props => (props.truncate === true && css`white-space: nowrap; overflow: hidden; text-overflow: ellipsis;`)}
   margin: 0;
`;

const StyledText = styled.p`
   ${styleCss}
`;

const TextBody = ({ title, tag, as, ...rest }) => { 
   //console.log("TextBody render", rest)
   return (
      <StyledText as={!as && tag ? tag : as} title={title} {...rest} />
   );
};

TextBody.propTypes = {
   as: PropTypes.string,
   tag: PropTypes.string,
   title: PropTypes.string,
   color: PropTypes.string,
   fontSize: PropTypes.number,
   fontWeight: PropTypes.number,
   backgroundColor: PropTypes.string,
   truncate: PropTypes.bool,
   isBold: PropTypes.bool,
   isInline: PropTypes.bool,
   isItalic: PropTypes.bool,
   display: PropTypes.string
};

TextBody.defaultProps = {
   title: '',
   color: '#333333',
   fontSize: 13,
   truncate: false,
   isBold: false,
   isInline: false,
   isItalic: false,
};

export default TextBody;