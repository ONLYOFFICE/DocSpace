import React from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isDesktop as isDesktopUtils } from "@docspace/components/utils/device";
import { isTablet } from "react-device-detect";

const tabletStyles = css`
  padding-top: 12px;
  .loader {
    width: 184px;
  }
`;

const StyledLoader = styled.div`
  padding-top: 8px;

  .loader {
    width: 273px;
  }

  @media (min-width: 600px) {
    ${tabletStyles}
  }

  ${isTablet &&
  css`
    ${tabletStyles}
  `}

  @media (min-width: 1025px) {
    .loader {
      width: 296px;
    }
  }
`;

const LoaderSectionHeader = () => {
  const heightLoader = isDesktopUtils() ? "29px" : "37px";
  return (
    <StyledLoader>
      <Loaders.Rectangle height={heightLoader} className="loader" />
    </StyledLoader>
  );
};

export default LoaderSectionHeader;
