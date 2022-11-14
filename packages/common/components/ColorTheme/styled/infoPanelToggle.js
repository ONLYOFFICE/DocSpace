import styled, { css } from "styled-components";
import { StyledInfoPanelToggleWrapper } from "@docspace/client/src/pages/Home/InfoPanel/Header/styles/common";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    .info-panel-toggle-bg {
      path {
        fill: ${theme.isBase
          ? $currentColorScheme.main.accent
          : theme.infoPanel.sectionHeaderToggleIconActive};
      }
    }
  `;

StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

export default styled(StyledInfoPanelToggleWrapper)(getDefaultStyles);
