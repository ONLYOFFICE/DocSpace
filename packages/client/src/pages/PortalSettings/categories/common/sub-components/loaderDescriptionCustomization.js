import React from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet } from "react-device-detect";

const tabletStyles = css`
  .description {
    width: 684px;
    padding-bottom: 20px;
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

  @media (min-width: 1024px) {
    .description {
      width: 700px;
    }
  }
`;

const LoaderDescriptionCustomization = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="40px" className="description" />
    </StyledLoader>
  );
};

export default LoaderDescriptionCustomization;
