import React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly, isTablet } from "react-device-detect";
import { mobile, tablet } from "@appserver/components/utils/device";

const StyledCatalogMainButton = styled.div`
  padding: 0px 20px 16px;
  max-width: 216px;
  @media ${tablet} {
    display: none;
  }

  @media ${mobile} {
    display: none;
  }

  ${isTablet &&
  css`
    display: none;
  `}

  ${isMobileOnly &&
  css`
    display: none;
  `}
`;

const CatalogMainButton = (props) => {
  return <StyledCatalogMainButton {...props} />;
};

CatalogMainButton.displayName = "CatalogMainButton";

export default CatalogMainButton;
