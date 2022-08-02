import styled from "styled-components";
import { desktop, tablet } from "@docspace/components/utils/device";

const StyledBox1 = styled.div`
  display: grid;
  grid-template-columns: 160px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 32px;

  @media (max-width: 428px) {
    grid-template-columns: 1fr;
    grid-template-rows: 1fr 1fr;
  }
  padding-bottom: 12px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 160px 36px;
  grid-row-gap: 12px;

  padding-bottom: 40px;

  @media (max-width: 428px) {
    padding-bottom: 32px;
  }
`;

const StyledBox3 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(9, 1fr);
  grid-row-gap: 8px;
  padding-bottom: 40px;
`;

const StyledBox4 = styled.div`
  display: grid;
  grid-template-columns: repeat(2, 200px);
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding-top: 40px;
  padding-bottom: 40px;

  @media ${desktop} {
    grid-template-columns: repeat(3, 200px);
  }
  @media ${tablet} {
    .row-content__tablet {
      display: none;
    }
  }
  @media (max-width: 428px) {
    grid-template-columns: 200px;
    .row-content__mobile {
      display: none;
    }
  }
`;

const StyledSpacer = styled.div`
  padding-bottom: 40px;
`;

export { StyledBox1, StyledBox2, StyledBox3, StyledBox4, StyledSpacer };
