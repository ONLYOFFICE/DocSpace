import styled, { css } from "styled-components";
import StyledPreparationPortalProgress from "../../StyledPreparationPortalProgress";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    .preparation-portal_progress-line {
      background: ${theme.isBase ? $currentColorScheme.main.accent : "#FFFFFF"};
    }
  `;

StyledPreparationPortalProgress.defaultProps = { theme: Base };

export default styled(StyledPreparationPortalProgress)(getDefaultStyles);
