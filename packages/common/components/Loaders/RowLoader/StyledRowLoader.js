import styled from "styled-components";

import { desktop } from "@docspace/components/utils/device";

const StyledRow = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 16px 22px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: ${(props) => props.gap || "8px"};
  margin-bottom: 32px;
  justify-items: center;
  align-items: center;
`;

const StyledBox1 = styled.div`
  .rectangle-content {
    width: 32px;
    height: 32px;
  }

  @media ${desktop} {
    .rectangle-content {
      width: 22px;
      height: 22px;
    }
  }
`;

const StyledBox2 = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 16px;
  grid-row-gap: 4px;
  justify-items: left;
  align-items: left;

  .first-row-content__mobile {
    width: 80%;
  }

  @media ${desktop} {
    grid-template-rows: 16px;
    grid-row-gap: 0;

    .first-row-content__mobile {
      width: 100%;
    }

    .second-row-content__mobile {
      width: 100%;
      display: none;
    }
  }
`;

export { StyledRow, StyledBox1, StyledBox2 };
