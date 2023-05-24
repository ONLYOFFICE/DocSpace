import styled, { css } from "styled-components";
import StyledText from "@docspace/components/text/styled-text";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isActive, theme }) =>
  $currentColorScheme &&
  css`
    color: ${isActive &&
    theme.isBase &&
    $currentColorScheme.main.accent} !important;

    &:hover {
      color: ${isActive &&
      theme.isBase &&
      $currentColorScheme.main.accent} !important;
    }
  `;

StyledText.defaultProps = { theme: Base };

export default styled(StyledText)(getDefaultStyles);
