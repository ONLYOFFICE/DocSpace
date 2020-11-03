import React from "react";
import styled from "styled-components";
import RectangleLoader from "./RectangleLoader";
import CircleLoader from "./CircleLoader";

const StyledTreeFolder = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 8px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 6px;
  margin-bottom: 24px;
`;

const StyledContainer = styled.div`
  margin-top: 48px;
`;

const TreeFolderLoader = () => {
  return (
    <div>
      <StyledTreeFolder>
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
      </StyledTreeFolder>

      <StyledTreeFolder>
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
      </StyledTreeFolder>

      <StyledTreeFolder>
        <CircleLoader radius="3" />
        <RectangleLoader width="100%" height="24" />
      </StyledTreeFolder>

      <StyledContainer>
        <RectangleLoader width="100%" height="48" />
      </StyledContainer>
    </div>
  );
};

export default TreeFolderLoader;
