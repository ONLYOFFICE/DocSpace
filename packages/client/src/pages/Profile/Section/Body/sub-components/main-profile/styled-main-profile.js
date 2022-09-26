import styled from "styled-components";
import {
  hugeMobile,
  smallTablet,
  desktop,
} from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  padding: 24px 24px 18px 24px;
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
  gap: 12px;

  @media ${smallTablet} {
    width: 100%;
    gap: 16px;
  }

  .rows-container {
    display: flex;
    flex-direction: column;
    gap: 16px;

    @media ${hugeMobile} {
      gap: 8px;
    }
  }

  .row {
    display: flex;
    align-items: center;
    gap: 8px;

    @media ${desktop} {
      height: 20px;
    }

    .field {
      display: flex;
      gap: 16px;

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
        gap: 2px;

        & > p {
          padding-left: 0;
          font-size: 12px !important;
          line-height: 16px !important;
        }
      }

      .label {
        min-width: 100%;
        max-width: 100%;
        font-size: 12px !important;
        line-height: 16px !important;
      }

      .edit-button {
        margin-left: auto;
      }
    }
  }
`;

export const StyledRow = styled.div`
  display: flex;
  gap: 16px;

  @media ${desktop} {
    height: 28px;
  }

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

    .label {
      font-weight: 600;
      color: ${(props) => props.theme.profile.main.textColor};
    }

    .combo {
      & > div {
        padding-left: 8px !important;
      }
    }
  }
`;
