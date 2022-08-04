import React from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet } from "react-device-detect";

const StyledLoader = styled.div`
  margin-top: -4px;

  .loader {
    padding-right: 4px;
  }

  @media (min-width: 600px) {
    margin-top: -9px;
  }

  ${isTablet &&
  css`
    margin-top: -9px;
  `}
`;

const LoaderSubmenu = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle width="100px" height="28px" className="loader" />
      <Loaders.Rectangle width="100px" height="28px" />
    </StyledLoader>
  );
};

export default LoaderSubmenu;
