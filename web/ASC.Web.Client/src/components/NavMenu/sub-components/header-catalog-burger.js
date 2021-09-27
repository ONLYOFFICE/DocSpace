import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import { mobile } from '@appserver/components/utils/device';
import MenuIcon from '@appserver/components/public/static/images/menu.react.svg';

const StyledIconBox = styled.div`
  display: none;

  @media ${mobile} {
    display: ${(props) => (props.isProduct && props.showCatalog ? 'flex' : 'none')};
    align-items: center;
  }
`;

const StyledMenuIcon = styled(MenuIcon)`
  width: 20px;
  height: 20px;
  fill: #657077;
  cursor: pointer;
`;

const HeaderCatalogBurger = (props) => {
  const { isProduct, showCatalog, onClick, ...rest } = props;

  return (
    <StyledIconBox isProduct={isProduct} showCatalog={showCatalog} onClick={onClick} {...rest}>
      <StyledMenuIcon />
    </StyledIconBox>
  );
};

HeaderCatalogBurger.propTypes = {
  isProduct: PropTypes.bool,
  onClick: PropTypes.func,
  showCatalog: PropTypes.bool,
};

export default React.memo(HeaderCatalogBurger);
