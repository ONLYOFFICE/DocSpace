import React from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet } from "react-device-detect";

const tabletStyles = css`
  .description {
    width: 100%;
    max-width: 700px;
    padding-bottom: 16px;
  }
`;

const StyledLoader = styled.div`
  @media (min-width: 600px) {
    ${tabletStyles}
  }

  ${isTablet &&
  css`
    ${tabletStyles}
  `}
`;

const LoaderDescriptionCustomization = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="40px" className="description" />
    </StyledLoader>
  );
};

export default LoaderDescriptionCustomization;
