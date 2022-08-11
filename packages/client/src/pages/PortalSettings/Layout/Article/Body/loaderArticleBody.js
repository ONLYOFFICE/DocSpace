import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { isTablet } from "react-device-detect";

const StyledLoader = styled.div`
  padding: 0px 16px 0px;

  .loader {
    display: block;
    width: 20px;
    padding-bottom: 16px;
  }

  @media (min-width: 1024px) {
    padding: 0px 20px 0px;

    .loader {
      width: 216px;
      padding-bottom: 12px;
    }
  }
`;

const LoaderArticleBody = () => {
  const heightLoader = isTabletUtils() || isTablet ? "20px" : "24px";
  return (
    <StyledLoader>
      <Loaders.Rectangle height={heightLoader} className="loader" />
      <Loaders.Rectangle height={heightLoader} className="loader" />
      <Loaders.Rectangle height={heightLoader} className="loader" />
      <Loaders.Rectangle height={heightLoader} className="loader" />
      <Loaders.Rectangle height={heightLoader} className="loader" />
    </StyledLoader>
  );
};

export default LoaderArticleBody;
