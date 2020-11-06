import React from "react";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import CircleLoader from "../CircleLoader/index";

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

const TreeFolderLoader = (props) => {
  return (
    <div>
      <StyledTreeFolder>
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
      </StyledTreeFolder>

      <StyledTreeFolder>
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
      </StyledTreeFolder>

      <StyledTreeFolder>
        <CircleLoader radius="3" height="32" {...props} />
        <RectangleLoader width="100%" height="24" {...props} />
      </StyledTreeFolder>

      <StyledContainer>
        <RectangleLoader width="100%" height="48" {...props} />
      </StyledContainer>
    </div>
  );
};

export default TreeFolderLoader;
