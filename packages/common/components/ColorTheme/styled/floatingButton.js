import styled, { css } from "styled-components";
import {
  StyledCircleWrap,
  StyledFloatingButton,
} from "@docspace/common/components/FloatingButton/StyledFloatingButton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, color, theme }) => css`
  background: ${color
    ? color
    : theme.isBase === true
    ? currentColorScheme.accentColor
    : theme.floatingButton.backgroundColor};

  ${StyledFloatingButton} {
    background: ${color
      ? color
      : theme.isBase === true
      ? currentColorScheme.accentColor
      : theme.floatingButton.backgroundColor};
  }
`;

StyledCircleWrap.defaultProps = { theme: Base };

export default styled(StyledCircleWrap)(getDefaultStyles);
