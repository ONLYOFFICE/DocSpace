import React from "react";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";

const StyledLoader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
  width: 300px;

  @media (max-width: 375px) {
    width: 100%;
  }
`;

const SectionLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle />
      <Loaders.Rectangle />
      <Loaders.Rectangle />
    </StyledLoader>
  );
};

export default SectionLoader;
