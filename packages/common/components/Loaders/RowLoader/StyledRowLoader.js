import styled from "styled-components";

import { desktop, isDesktop } from "@docspace/components/utils/device";

const StyledRow = styled.div`
  width: 100%;
  height: 32px;
  display: grid;
  grid-template-columns: 32px 1fr 16px;
  grid-template-rows: 1fr;
  grid-column-gap: ${isDesktop() ? "8px" : "12px"};
  margin-bottom: ${isDesktop() ? "16px" : "26px"};
  justify-items: center;
  align-items: center;
`;

const StyledBox = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 16px;
  grid-row-gap: 2px;
  justify-items: left;
  align-items: left;
  box-sizing: border-box;
  padding-right: ${isDesktop() ? "8px" : "12px"};

  .first-row-content__mobile {
    max-width: 384px;
    height: 16px;
  }

  .second-row-content__mobile {
    max-width: 176px;
    width: 50%;
  }

  @media ${desktop} {
    grid-template-rows: 16px;
    grid-row-gap: 0;

    .first-row-content__mobile {
      max-width: none;
      height: 20px;
      width: 100%;
    }

    .second-row-content__mobile {
      display: none;
    }
  }
`;

export { StyledRow, StyledBox };
