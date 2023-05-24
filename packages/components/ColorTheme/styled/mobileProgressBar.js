import styled, { css } from "styled-components";
import { StyledBar } from "@docspace/components/main-button-mobile/styled-main-button";

const getDefaultStyles = ({ $currentColorScheme, theme, error }) =>
  $currentColorScheme &&
  css`
    background: ${error
      ? theme.mainButtonMobile.bar.errorBackground
      : theme.isBase
      ? $currentColorScheme.main.accent
      : "#FFFFFF"};
  `;

export default styled(StyledBar)(getDefaultStyles);
