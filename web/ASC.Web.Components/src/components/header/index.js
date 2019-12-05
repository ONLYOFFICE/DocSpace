import React from "react";
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import commonTextStyles from '../text/common-text-styles';

const fontSize = css`
      ${props =>
      (props.type === 'menu' && 27) ||
      (props.type === 'content' && 21)
   }
   `;

const styles = css`
   margin: 0;
   line-height: 56px;
   font-size: ${fontSize}px;
   font-weight: 700;
   ${props => props.isInline && css`display: inline-block;`}
`;

const StyledHeader = styled.h1`
      ${styles};
      ${commonTextStyles};
   `;

const Header = ({ title, tag, as, children, ...rest }) => {
   //console.log("Header render");
   return (
      <StyledHeader as={!as && tag ? tag : as} title={title} {...rest}>{children}</StyledHeader>
   );
};

Header.propTypes = {
   as: PropTypes.string,
   tag: PropTypes.string,
   // children: PropTypes.string,
   color: PropTypes.string,
   title: PropTypes.string,
   truncate: PropTypes.bool,
   isInline: PropTypes.bool,
   type: PropTypes.oneOf(['menu', 'content']),
};

Header.defaultProps = {
   color: '#333333',
   title: '',
   truncate: false,
   isInline: false,
};

export default Header;