import React from "react";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";

const StyledLoader = styled.div``;

const LoaderLngTZSettings = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle />
    </StyledLoader>
  );
};

export default LoaderLngTZSettings;
