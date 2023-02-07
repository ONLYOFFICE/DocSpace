import React from "react";
import styled from "styled-components";

import RectangleLoader from "../../RectangleLoader/RectangleLoader";

const StyledSeveralItemsLoader = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
`;

const SeveralItemsLoader = () => {
  return (
    <StyledSeveralItemsLoader>
      <RectangleLoader width={"96px"} height={"96px"} borderRadius={"6px"} />
    </StyledSeveralItemsLoader>
  );
};

export default SeveralItemsLoader;
