import styled, { css } from "styled-components";
import {
  hugeMobile,
  mobile,
  smallTablet,
  tablet,
} from "@docspace/components/utils/device";
import Text from "@docspace/components/text";

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

export const StyledAvatarWrapper = styled.div`
  display: flex;

  @media ${smallTablet} {
    width: 100%;
    justify-content: center;
  }

  .badges-wrapper {
    display: none;

    @media ${smallTablet} {
      display: flex;
      position: fixed;
      right: 16px;
    }
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
  padding-top: 5px;

  @media ${tablet} {
    gap: 7px;
  }

  .rows-container {
    display: grid;
    grid-template-columns: minmax(75px, auto) 1fr;
    gap: 24px;
    max-width: 100%;

    .profile-block {
      display: flex;
      flex-direction: column;

      .profile-block-field {
        display: flex;
        gap: 8px;
        height: 20px;
        align-items: center;
        line-height: 20px;
      }

      .sso-badge {
        margin-left: 18px;
      }

      .profile-block-password {
        margin-top: 16px;
      }

      .email-container {
        margin-top: 16px;

        .send-again-desktop {
          display: flex;
        }
      }
      .language-combo-box-wrapper {
        display: flex;
        height: 28px;
        align-items: center;
        margin-top: 11px;

        @media ${tablet} {
          height: 36px;
          margin-top: 7px;
        }

        .language-combo-box {
          .combo-button {
            margin-left: -16px;
          }
        }
      }
    }
  }

  .mobile-profile-block {
    display: none;
  }

  .edit-button {
    min-width: 12px;

    svg path {
      fill: ${(props) => props.theme.isBase && `#657077`};
    }
  }

  .email-edit-container {
    display: flex;
    align-items: center;
    padding-right: 16px;
    line-height: 20px;

    .email-text-container {
      ${(props) =>
        props.withActivationBar &&
        css`
          color: ${props.theme.profile.main.pendingEmailTextColor};
        `}
    }

    .email-edit-button {
      display: block;
      padding-left: 8px;
    }
  }

  .send-again-container {
    display: flex;
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

  .profile-language {
    display: flex;
  }

  @media ${smallTablet} {
    .rows-container {
      display: none;
    }

    .mobile-profile-block {
      display: flex;
      flex-direction: column;
      gap: 16px;
      max-width: 100%;

      .mobile-profile-row {
        gap: 8px;
        background: ${(props) => props.theme.profile.main.mobileRowBackground};
        padding: 12px 16px;
        border-radius: 6px;
        display: flex;
        align-items: center;
        line-height: 20px;
        max-width: 100%;

        .mobile-profile-field {
          display: flex;
          align-items: baseline;
          max-width: calc(100% - 28px);
          flex-direction: column;
          gap: 2px;
        }

        .mobile-profile-label {
          min-width: 100%;
          max-width: 100%;
          font-size: 12px !important;
          line-height: 16px !important;
          white-space: nowrap;
          color: rgb(163, 169, 174);
        }

        .mobile-profile-label-field {
          padding-left: 0px;
          max-width: 100%;
          font-size: 12px !important;
          line-height: 16px;
        }

        .email-container {
          padding-left: 0px;

          display: flex;
          flex-wrap: wrap;
          align-items: baseline;
        }

        .edit-button {
          margin-left: auto;
          min-width: 12px;

          svg path {
            fill: ${(props) => props.theme.isBase && `#657077`};
          }
        }

        .mobile-profile-password {
          max-width: 100%;
          font-size: 12px !important;
          line-height: 16px !important;
        }
      }

      .mobile-language {
        display: flex;
        width: 100%;
        flex-direction: column;
        gap: 4px;

        @media ${mobile} {
          margin-top: 8px;
        }

        .mobile-profile-label {
          display: flex;
          align-items: center;
          gap: 4px;
          min-width: 75px;
          max-width: 75px;
          white-space: nowrap;
        }
      }

      @media ${hugeMobile} {
        gap: 8px;
      }
    }
  }
`;

export const StyledLabel = styled(Text)`
  display: block;
  align-items: center;
  gap: 4px;

  min-width: 100%;
  width: 100%;
  line-height: 20px;
  white-space: nowrap;
  color: ${(props) => props.theme.profile.main.descriptionTextColor};

  overflow: hidden;
  text-overflow: ellipsis;

  margin-top: ${({ marginTopProp }) => (marginTopProp ? marginTopProp : 0)};
`;
