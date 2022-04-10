import React from "react";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";

const StyledLoader = styled.div`
  @media (min-width: 600px) {
    .description {
      width: 684px;
      padding-bottom: 20px;
    }
  }
  @media (min-width: 1024px) {
    .description {
      width: 700px;
    }
  }
`;

const loaderDescriptionCustomization = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="description" />
    </StyledLoader>
  );
};

export default loaderDescriptionCustomization;
