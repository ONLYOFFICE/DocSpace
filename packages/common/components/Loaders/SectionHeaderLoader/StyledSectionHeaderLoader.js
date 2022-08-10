import styled from "styled-components";

import { tablet, mobile } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 100px 0fr 42px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  margin-top: 20px;
  margin-bottom: 18px;

  @media ${mobile}, ${tablet} {
    margin-top: 23px;
    grid-template-columns: 100px 1fr 42px;
  }
`;

const StyledBox1 = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 17px 67px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 17px 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
`;

const StyledSpacer = styled.div``;

export { StyledContainer, StyledBox1, StyledBox2, StyledSpacer };
