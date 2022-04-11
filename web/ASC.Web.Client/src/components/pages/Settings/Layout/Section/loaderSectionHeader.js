import React from "react";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";
import { isDesktop as isDesktopUtils } from "@appserver/components/utils/device";
import { isTablet } from "react-device-detect";

const StyledLoader = styled.div`
  padding-top: 8px;

  .loader {
    width: 273px;
  }

  @media (min-width: 600px) {
    padding-top: 12px;
    .loader {
      width: 184px;
    }
  }

  ${isTablet &&
  css`
    padding-top: 12px;
    .loader {
      width: 184px;
    }
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
