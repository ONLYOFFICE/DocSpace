import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@appserver/components/scrollbar";
import ArrowRightIcon from "../../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { Base } from "@appserver/components/themes";

const menuHeight = "48px";
const sectionHeight = "50px";
const paddingSectionWrapperContent = "22px";
const saveCancelButtons = "56px";
const flex = "4px";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.studio.settings.common.arrowColor};
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

  .field-container-flex {
    display: flex;
    justify-content: space-between;
    margin-top: 8px;
    margin-bottom: 12px;
  }

  .toggle {
    position: inherit;
    grid-gap: inherit;
  }

  .field-title {
    font-weight: 600;
    line-height: 20px;
  }

  .errorText {
    position: absolute;
    font-size: 10px;
    color: #f21c0e;
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

    .settings-block-description {
      line-height: 20px;
      color: #657077;
      padding-bottom: 12px;
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
