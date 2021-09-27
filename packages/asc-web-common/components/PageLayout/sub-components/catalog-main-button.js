import React from 'react';
import styled from 'styled-components';
import { tablet, mobile } from '@appserver/components/utils/device';

const StyledCatalogMainButton = styled.div`
  padding: 0px 20px 16px;

  @media ${tablet} {
    display: ${(props) => (props.showText ? 'block' : 'none')};
    padding: 0 20px 16px;
  }

  @media ${mobile} {
    padding: 16px 20px 16px;
  }
`;

const CatalogMainButton = (props) => {
  return <StyledCatalogMainButton {...props} />;
};

CatalogMainButton.displayName = 'CatalogMainButton';

export default CatalogMainButton;
