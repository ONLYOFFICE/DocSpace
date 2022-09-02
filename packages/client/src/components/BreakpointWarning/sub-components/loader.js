import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  padding: 0 28px;

  .img {
    padding-bottom: 32px;
  }

  .loader-text {
    padding-bottom: 8px;
  }

  .block {
    display: block;
  }
`;

const Loader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="72px" width="72px" className="img block" />
      <Loaders.Rectangle
        height="44px"
        width="287px"
        className="loader-text block"
      />
      <Loaders.Rectangle height="32px" width="287px" className="block" />
    </StyledLoader>
  );
};

export default Loader;
