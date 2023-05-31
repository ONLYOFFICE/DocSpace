import styled, { css } from "styled-components";

import { StyledInputGroup } from "@docspace/components/input-block/styled-input-block";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, hasError, theme }) =>
  $currentColorScheme &&
  theme.isBase &&
  css`
    :focus-within {
      border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
      $currentColorScheme.main.accent};
    }
  `;

StyledInputGroup.defaultProps = {
  theme: Base,
};

export default styled(StyledInputGroup)(getDefaultStyles);
