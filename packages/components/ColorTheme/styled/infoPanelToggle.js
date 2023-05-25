import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import StyledInfoPanelToggleWrapper from "./sub-components/StyledWrapper";

const getDefaultStyles = ({ $currentColorScheme, isInfoPanelVisible }) =>
  $currentColorScheme &&
  isInfoPanelVisible &&
  css`
    .info-panel-toggle-bg {
      path {
        fill: ${$currentColorScheme.main.accent};
      }
      &:hover {
        path {
          fill: ${$currentColorScheme.main.accent};
        }
      }
    }
  `;

StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

export default styled(StyledInfoPanelToggleWrapper)(getDefaultStyles);
