import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

   const fontSize = css`
      ${props =>
         (props.size === 'big' && 23) ||
         (props.size === 'medium' && 19) ||
         (props.size === 'small' && 15)
      }
   `;

   const styles = css`
      font-family: 'Open Sans',sans-serif,Arial;
      font-size: ${fontSize}px;
      font-weight: 600;
      color: ${props => props.color};
      text-align: left;
      ${props => (props.truncate && css`white-space: nowrap; overflow: hidden; text-overflow: ellipsis;`)}
      ${props => props.isInline && css`display: inline-block;`}
   `

   const StyledHeadline = styled.h1`
      ${styles}
   `;

   const Text = ({ title, tag, as, ...rest }) => {
      return (
         <StyledHeadline as={!as && tag ? tag : as} title={title} {...rest}></StyledHeadline>
      );
   };

   Text.propTypes = {
      as: PropTypes.string,
      tag: PropTypes.string,
      color: PropTypes.string,
      title: PropTypes.string,
      truncate: PropTypes.bool,
      isInline: PropTypes.bool,
      size: PropTypes.oneOf(['big', 'medium', 'small']),
   };

   Text.defaultProps = {
      color: '#333333',
      title: '',
      truncate: false,
      isInline: false,
      size: 'big'
   };

   export default Text;
