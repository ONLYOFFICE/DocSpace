import styled, { css } from "styled-components";
import StyledTextInput from "@docspace/components/text-input/styled-text-input";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({
  $currentColorScheme,
  hasError,
  hasWarning,
  isDisabled,
  theme,
}) =>
  $currentColorScheme &&
  theme.isBase &&
  css`
    :focus {
      border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
      (hasWarning && theme.input.focusWarningBorderColor) ||
      (isDisabled && theme.input.focusDisabledBorderColor) ||
      $currentColorScheme.main.accent};
    }
  `;

StyledTextInput.defaultProps = { theme: Base };

export default styled(StyledTextInput)(getDefaultStyles);
