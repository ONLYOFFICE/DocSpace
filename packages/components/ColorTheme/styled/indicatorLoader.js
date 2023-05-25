import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import StyledWrapper from "./sub-components/StyledWrapper";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    #ipl-progress-indicator {
      background-color: ${theme.isBase
        ? $currentColorScheme.main.accent
        : "#FFFFFF"};

      &:hover {
        background-color: ${theme.isBase
          ? $currentColorScheme.main.accent
          : "#FFFFFF"};
      }
    }
  `;
StyledWrapper.defaultProps = { theme: Base };
export default styled(StyledWrapper)(getDefaultStyles);
