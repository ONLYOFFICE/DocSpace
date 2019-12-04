import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';


const fontSize = css`
      ${props =>
      (props.headlineType === 'MenuHeader' && 27) ||
      (props.headlineType === 'ContentHeader' && 21)
   }
   `;

const styles = css`
   font-family: 'Open Sans',sans-serif,Arial;
   margin: 0;
   line-height: 56px;
   font-size: ${fontSize}px;
   font-weight: 700;
   color: ${props => props.color};
   text-align: left;
   ${props => (props.truncate && css`white-space: nowrap; overflow: hidden; text-overflow: ellipsis;`)}
   ${props => props.isInline && css`display: inline-block;`}
`;

const StyledHeadline = styled.h1`
      ${styles};
   `;

const Text = ({ title, tag, as, children, ...rest }) => {
   //console.log("Text.Header render");
   return (
      <StyledHeadline as={!as && tag ? tag : as} title={title} {...rest}>{children}</StyledHeadline>
   );
};

Text.propTypes = {
   as: PropTypes.string,
   tag: PropTypes.string,
   children: PropTypes.string,
   color: PropTypes.string,
   title: PropTypes.string,
   truncate: PropTypes.bool,
   isInline: PropTypes.bool,
   headlineType: PropTypes.oneOf(['MenuHeader', 'ContentHeader']),
};

Text.defaultProps = {
   color: '#333333',
   title: '',
   truncate: false,
   isInline: false
};

export default Text;