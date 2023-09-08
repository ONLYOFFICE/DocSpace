import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@docspace/components/scrollbar";
import ArrowRightIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";
import { UnavailableStyles } from "../../../utils/commonSettingsStyles";
import { mobile } from "@docspace/components/utils/device";

const menuHeight = "48px";
const sectionHeight = "50px";
const paddingSectionWrapperContent = "22px";
const saveCancelButtons = "56px";
const flex = "4px";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${props => props.theme.client.settings.common.arrowColor};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

const StyledScrollbar = styled(Scrollbar)`
  height: calc(
    100vh -
      (
        ${menuHeight} + ${sectionHeight} + ${paddingSectionWrapperContent} +
          ${saveCancelButtons} + ${flex}
      )
  ) !important;
  width: 100% !important;
`;

const StyledSettingsComponent = styled.div`
  .dns-setting_helpbutton {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 4px;
          `
        : css`
            margin-right: 4px;
          `}
  }

  .paid-badge {
    cursor: auto;
  }

  .dns-textarea {
    textarea {
      color: ${props => props.theme.text.disableColor};
    }
    ${props => props.standalone && "margin-top: 14px"};
  }

  .combo-button-label {
    max-width: 100%;
    font-weight: 400;
  }

  .toggle {
    position: inherit;
    grid-gap: inherit;
  }

  .errorText {
    position: absolute;
    font-size: 10px;
    color: #f21c0e;
  }

  .settings-block-description {
    line-height: 20px;
    color: ${props => props.theme.client.settings.security.descriptionColor};
    padding-bottom: 12px;
  }

  .send-request-button {
    height: 40px;
  }

  .combo-box-settings {
    .combo-button {
      justify-content: space-between !important;
    }
  }
  .settings-dns_toggle-button {
    .toggle-button-text {
      font-weight: 600;
      font-size: 14px;
    }
    svg {
      margin-top: 1px;
    }
  }

  .link-learn-more {
    display: block;
    margin: 4px 0 16px 0;
    font-weight: 600;
  }

  .category-item-description {
    p,
    a {
      color: ${(props) => props.theme.client.settings.common.descriptionColor};
    }

    @media ${mobile} {
      padding-right: 8px;
    }
  }

  @media (max-width: 599px) {
    ${props =>
      props.hasScroll &&
      css`
        width: ${isMobileOnly ? "100vw" : "calc(100vw - 52px)"};
        ${props =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                right: -16px;
              `
            : css`
                left: -16px;
              `}
        position: relative;

        .settings-block {
          width: ${isMobileOnly ? "calc(100vw - 32px)" : "calc(100vw - 84px)"};
          max-width: none;
          ${props =>
            props.theme.interfaceDirection === "rtl"
              ? css`
                  padding-right: 16px;
                `
              : css`
                  padding-left: 16px;
                `}
        }
      `}

    .send-request-container {
      box-sizing: border-box;
      position: absolute;
      bottom: 0;
      ${props =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              right: 0;
            `
          : css`
              left: 0;
            `}
      padding: 16px;
      width: 100%;
    }

    .send-request-button {
      width: 100%;
    }
  }

  @media (min-width: 600px) {
    .settings-block {
      max-width: 350px;
      height: auto;
      margin-top: 0px;
    }

    .settings-block-description {
      display: none;
    }
  }

  @media (min-width: 1024px) {
    .send-request-button {
      height: 32px;
    }
  }

  @media (orientation: landscape) and (max-width: 600px) {
    ${isMobileOnly &&
    css`
      .settings-block {
        height: auto;
      }
    `}
  }
  ${props => !props.isSettingPaid && UnavailableStyles}
`;

export { StyledSettingsComponent, StyledScrollbar, StyledArrowRightIcon };
