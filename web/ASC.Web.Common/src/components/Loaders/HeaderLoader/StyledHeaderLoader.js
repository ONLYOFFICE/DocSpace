import styled from "styled-components";

const StyledHeader = styled.div`
  height: 56px;
  align-items: center;
  display: grid;
  grid-template-columns: 24px 168px 1fr 36px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding-left: 16px;
  padding-right: 16px;
`;

const StyledSpacer = styled.div``;

export { StyledHeader, StyledSpacer };
