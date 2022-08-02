import styled from "styled-components";
import { tablet } from "@docspace/components/utils/device";

const StyledHeader = styled.div`
  height: 48px;
  align-items: center;
  display: grid;
  grid-template-columns: 208px 1fr 36px;

  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding-left: 20px;
  padding-right: 20px;
  @media ${tablet} {
    padding-left: 16px;
    padding-right: 16px;
  }
`;

const StyledSpacer = styled.div``;

export { StyledHeader, StyledSpacer };
