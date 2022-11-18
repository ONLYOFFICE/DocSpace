import styled from "styled-components";

import { tablet, mobile } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 140px 0fr 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
  margin-top: 22px;
  margin-bottom: 23px;
  align-items: center;

  @media ${tablet} {
    margin-top: 16px;
    margin-bottom: 25px;
    grid-template-columns: 163px 1fr 17px;
  }

  @media ${mobile} {
    margin-top: 14px;
    margin-bottom: 23px;
    grid-template-columns: 140px 1fr 17px;
  }
`;

const StyledBox1 = styled.div`
  width: 140px;
  height: 24px;

  @media ${tablet} {
    width: 163px;
    height: 28px;
  }

  @media ${mobile} {
    width: 140px;
    height: 24px;
  }
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 17px 17px;
  grid-template-rows: 1fr;
`;

const StyledSpacer = styled.div``;

export { StyledContainer, StyledBox1, StyledBox2, StyledSpacer };
