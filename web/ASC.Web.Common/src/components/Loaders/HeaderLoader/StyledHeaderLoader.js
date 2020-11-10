import styled from "styled-components";

const StyledHeader = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 24px 168px 1fr 24px 36px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding: 16px;
`;

const StyledSpacer = styled.div``;

export { StyledHeader, StyledSpacer };
