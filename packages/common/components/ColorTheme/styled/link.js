import styled, { css } from "styled-components";
import StyledText from "@docspace/components/link/styled-link";

const getDefaultStyles = ({ $currentColorScheme, noHover, type }) =>
  $currentColorScheme &&
  css`
    color: ${$currentColorScheme.accentColor};

    &:hover {
      color: ${!noHover && $currentColorScheme.accentColor};
      text-decoration: underline;
    }
  `;

export default styled(StyledText)(getDefaultStyles);
