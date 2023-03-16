import styled, { css } from "styled-components";
import {
  hugeMobile,
  smallTablet,
  desktop,
  tablet,
} from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  width: 100%;
  max-width: 100%;

  display: flex;
  padding: 24px 24px 18px 24px;
  gap: 40px;
  background: ${(props) => props.theme.profile.main.background};
  border-radius: 12px;

  box-sizing: border-box;

  .avatar {
    min-width: 124px;
  }

  @media ${smallTablet} {
    background: none;
    flex-direction: column;
    gap: 24px;
    align-items: center;
    padding: 0;
  }
`;

export const StyledInfo = styled.div`
  width: 100%;
  max-width: 100%;

  box-sizing: border-box;

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  display: flex;
  flex-direction: column;
  gap: 11px;

  @media ${tablet} {
    gap: 7px;
  }

  padding-top: 5px;

  @media ${smallTablet} {
    width: 100%;
    gap: 16px;
  }

  .label {
    min-width: 75px;
    max-width: 75px;
    white-space: nowrap;
    color: ${(props) => props.theme.profile.main.descriptionTextColor};
  }

  .rows-container {
    display: flex;
    flex-direction: column;
    gap: 16px;

    max-width: 100%;

    @media ${hugeMobile} {
      gap: 8px;
    }
  }

  .row {
    display: flex;
    align-items: baseline;
    gap: 8px;
    line-height: 20px;
    max-width: 100%;

    @media ${desktop} {
      height: 20px;
    }

    @media ${smallTablet} {
      align-items: center;
    }

    .field {
      display: flex;
      gap: 16px;
      align-items: baseline;
      max-width: calc(100% - 28px);

      & > p {
        padding-left: 8px;
      }
    }

    .email-text-container {
      ${(props) =>
        props.withActivationBar &&
        css`
          color: ${props.theme.profile.main.pendingEmailTextColor};
        `}
    }

    .send-again-container {
      flex-grow: 1;

      max-width: 50%;
      cursor: pointer;

      align-items: center;

      cursor: pointer;

      height: 18px;

      .send-again-text {
        margin-left: 5px;

        line-height: 15px;

        color: ${(props) => props.currentColorScheme.main.accent};

        border-bottom: 1px solid
          ${(props) => props.currentColorScheme.main.accent};

        margin-top: 2px;
      }

      .send-again-icon {
        display: block;

        width: 12px;
        height: 12px;

        display: flex;
        align-items: center;
        justify-content: center;

        div {
          width: 12px;
          height: 12px;
        }

        svg {
          width: 12px;
          height: 12px;

          path {
            fill: ${(props) => props.currentColorScheme.main.accent};
          }
        }
      }
    }

    .send-again-desktop {
      display: flex;
    }

    .send-again-mobile {
      display: none;
    }

    .edit-button {
      min-width: 12px;

      svg path {
        fill: ${(props) => props.theme.isBase && `#657077`};
      }
    }

    .email-edit-button {
      display: block;
      padding-left: 8px;
    }

    .email-edit-container {
      display: flex;
      align-items: center;
      padding-right: 16px;
    }

    .email-container {
      padding-left: 8px;
      display: flex;
      flex-wrap: wrap;
      align-items: baseline;
    }

    .email-edit-button-mobile {
      display: none;
    }

    @media ${smallTablet} {
      gap: 8px;
      background: ${(props) => props.theme.profile.main.background};
      padding: 12px 16px;
      border-radius: 6px;

      .field {
        flex-direction: column;
        gap: 2px;

        .email-container {
          padding-left: 0px;
        }

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

      .email-edit-button-mobile {
        display: block;
      }

      .email-edit-button {
        display: none;
      }

      .edit-button {
        margin-left: auto;
        min-width: 12px;
      }

      .send-again-desktop {
        display: none;

        margin-left: 8px;
      }

      .send-again-mobile {
        display: flex;
      }
    }
  }
`;

export const StyledRow = styled.div`
  display: flex;
  gap: 16px;

  @media ${desktop} {
    height: 28px;
    align-items: center;
  }

  .label {
    display: flex;
    align-items: center;
    gap: 4px;
    min-width: 75px;
    max-width: 75px;
    white-space: nowrap;
  }

  .language-combo-box {
    margin-left: -8px;
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

    .language-combo-box {
      margin-left: 0;
    }
  }
`;
