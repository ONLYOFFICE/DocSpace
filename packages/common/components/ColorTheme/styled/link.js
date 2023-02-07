import styled, { css } from "styled-components";
import StyledText from "@docspace/components/link/styled-link";

const getDefaultStyles = ({ $currentColorScheme, noHover }) =>
  $currentColorScheme &&
  css`
    color: ${$currentColorScheme.main.accent};

    &:hover {
      color: ${!noHover && $currentColorScheme.main.accent};
      text-decoration: underline;
    }
  `;

export default styled(StyledText)(getDefaultStyles);
