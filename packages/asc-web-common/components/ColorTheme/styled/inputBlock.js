import styled, { css } from "styled-components";

import { StyledInputGroup } from "@appserver/components/input-block/styled-input-block";
import Base from "@appserver/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, hasError, theme }) => css`
  :focus-within {
    border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
    (theme.isBase === true
      ? currentColorScheme.accentColor
      : theme.inputBlock.borderColor)};
  }
`;

StyledInputGroup.defaultProps = {
  theme: Base,
};

export default styled(StyledInputGroup)(getDefaultStyles);
