import styled, { css } from "styled-components";

import { StyledInputGroup } from "@docspace/components/input-block/styled-input-block";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, hasError, theme }) =>
  $currentColorScheme &&
  css`
    :focus-within {
      border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
      (theme.isBase
        ? $currentColorScheme.accentColor
        : theme.inputBlock.borderColor)};
    }
  `;

StyledInputGroup.defaultProps = {
  theme: Base,
};

export default styled(StyledInputGroup)(getDefaultStyles);
