import React from "react";
import styled from "styled-components";
import RectangleLoader from "./RectangleLoader";

const StyledHead = styled.div`
  margin-top: 13px;
  margin-bottom: 10px;
`;

const HeadLoader = () => {
  return (
    <StyledHead>
      <RectangleLoader />
    </StyledHead>
  );
};

export default HeadLoader;
