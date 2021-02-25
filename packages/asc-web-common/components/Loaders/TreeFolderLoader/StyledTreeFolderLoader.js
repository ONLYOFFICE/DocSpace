import styled from "styled-components";

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

export { StyledTreeFolder, StyledContainer };
