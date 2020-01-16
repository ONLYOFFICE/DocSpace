import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import commonTextStyles from '../text/common-text-styles';

const fontSize = css`
      ${props =>
      (props.size === 'xlarge' && 27) ||
      (props.size === 'large' && 23) ||
      (props.size === 'medium' && 21) ||
      (props.size === 'small' && 19) ||
      (props.size === 'xsmall' && 15)
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

const Heading = ({ level, color, ...rest }) => {
   return (
      <StyledHeading
         as={`h${level}`}
         colorProp={color}
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
   size: PropTypes.oneOf(['xsmall', 'small', 'medium', 'large', 'xlarge']),
   className: PropTypes.string,
};

Heading.defaultProps = {
   color: '#333333',
   title: null,
   truncate: false,
   isInline: false,
   size: 'large',
   level: 1,
   className: ''
};

export default Heading;
