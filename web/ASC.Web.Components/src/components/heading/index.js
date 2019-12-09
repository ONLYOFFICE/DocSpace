import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import commonTextStyles from '../text/common-text-styles';

const fontSize = css`
      ${props =>
      (props.size === 'big' && 23) ||
      (props.size === 'medium' && 19) ||
      (props.size === 'small' && 15)
   }
   `;

const styles = css`
      font-size: ${fontSize}px;
      font-weight: 600;
      ${props => props.isInline && css`display: inline-block;`}
   `

const StyledHeading = styled.h1`
      ${styles};
      ${commonTextStyles};
   `;

const Heading = ({ title, level, color, ...rest }) => {
   return (
      <StyledHeading
         as={`h${level}`}
         level={+level}
         title={title}
         color={color}
         {...rest}>
      </StyledHeading>
   );
};

Heading.propTypes = {
   level: PropTypes.oneOf([1, 2, 3, 4, 5, 6]),
   color: PropTypes.string,
   title: PropTypes.string,
   truncate: PropTypes.bool,
   isInline: PropTypes.bool,
   size: PropTypes.oneOf(['big', 'medium', 'small']),
};

Heading.defaultProps = {
   color: '#333333',
   title: '',
   truncate: false,
   isInline: false,
   size: 'big',
   level: 1
};

export default Heading;
