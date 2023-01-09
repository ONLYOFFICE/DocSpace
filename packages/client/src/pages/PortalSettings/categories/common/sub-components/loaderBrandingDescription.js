import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import styled from "styled-components";

const StyledLoader = styled.div`
  padding-bottom: 18px;

  .loader-description {
    display: block;
    padding-bottom: 8px;
  }
`;

const LoaderBrandingDescription = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle
        width="700px"
        height="16px"
        className="loader-description"
      />
      <Loaders.Rectangle width="93px" height="16px" />
    </StyledLoader>
  );
};

export default LoaderBrandingDescription;
