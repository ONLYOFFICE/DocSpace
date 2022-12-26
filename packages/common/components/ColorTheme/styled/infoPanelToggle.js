import styled, { css } from "styled-components";
import StyledInfoPanelToggleWrapper from "../../StyledInfoPanelToggleWrapper";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
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
