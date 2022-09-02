import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@docspace/components/scrollbar";
import ArrowRightIcon from "@docspace/client/public/images/arrow.right.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";

const menuHeight = "48px";
const sectionHeight = "50px";
const paddingSectionWrapperContent = "22px";
const saveCancelButtons = "56px";
const flex = "4px";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.client.settings.common.arrowColor};
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
    color: #657077;
    padding-bottom: 12px;
  }

  .send-request-button {
    height: 40px;
  }

  @media (max-width: 599px) {
    ${(props) =>
      props.hasScroll &&
      css`
        width: ${isMobileOnly ? "100vw" : "calc(100vw - 52px)"};
        left: -16px;
        position: relative;

        .settings-block {
          width: ${isMobileOnly ? "calc(100vw - 32px)" : "calc(100vw - 84px)"};
          max-width: none;
          padding-left: 16px;
        }
      `}

    .send-request-container {
      box-sizing: border-box;
      position: absolute;
      bottom: 0;
      left: 0;
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
`;

export { StyledSettingsComponent, StyledScrollbar, StyledArrowRightIcon };
