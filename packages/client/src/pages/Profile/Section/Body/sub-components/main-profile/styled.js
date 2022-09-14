import styled from "styled-components";
import { smallTablet } from "@docspace/components/utils/device";

export const StyledRow = styled.div`
  display: flex;
  gap: 24px;

  .combo {
    & > div {
      padding: 0 !important;
      justify-content: flex-start !important;
    }
  }

  .label {
    display: flex;
    align-items: center;
    gap: 4px;
    min-width: 75px;
    max-width: 75px;
    white-space: nowrap;
  }

  @media ${smallTablet} {
    width: 100%;
    flex-direction: column;
    gap: 4px;

    .combo {
      & > div {
        padding-left: 8px !important;
      }
    }
  }
`;
