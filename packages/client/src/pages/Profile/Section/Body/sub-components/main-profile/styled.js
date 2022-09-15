import styled from "styled-components";
import { smallTablet } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  padding: 24px;
  gap: 40px;
  background: ${(props) => props.theme.profile.main.background};
  border-radius: 12px;

  @media ${smallTablet} {
    background: none;
    flex-direction: column;
    gap: 24px;
    align-items: center;
    padding: 0;
  }
`;

export const StyledInfo = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  @media ${smallTablet} {
    width: 100%;
  }

  .row {
    display: flex;
    align-items: center;
    gap: 8px;

    .field {
      display: flex;
      gap: 24px;

      & > p {
        padding-left: 8px;
      }
    }

    .label {
      min-width: 75px;
      max-width: 75px;
      white-space: nowrap;
    }

    @media ${smallTablet} {
      gap: 8px;
      background: ${(props) => props.theme.profile.main.background};
      padding: 12px 16px;
      border-radius: 6px;

      .field {
        flex-direction: column;
        gap: 4px;

        & > p {
          padding-left: 0;
        }
      }

      .label {
        min-width: 100%;
        max-width: 100%;
      }

      .edit-button {
        margin-left: auto;
      }
    }
  }
`;

export const StyledRow = styled.div`
  display: flex;
  gap: 24px;

  .combo {
    & > div {
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
