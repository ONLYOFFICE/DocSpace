import styled, { css } from "styled-components";
import { StyledBodyPreparationPortal } from "@docspace/client/src/pages/PreparationPortal/StyledPreparationPortal";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    .preparation-portal_progress-line {
      background: ${theme.isBase ? $currentColorScheme.main.accent : "#FFFFFF"};
    }
  `;

StyledBodyPreparationPortal.defaultProps = { theme: Base };

export default styled(StyledBodyPreparationPortal)(getDefaultStyles);
