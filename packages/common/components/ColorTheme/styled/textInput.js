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
  css`
    :focus {
      border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
      (hasWarning && theme.input.focusWarningBorderColor) ||
      (isDisabled && theme.input.focusDisabledBorderColor) ||
      (theme.isBase
        ? $currentColorScheme.accentColor
        : theme.input.focusBorderColor)};
    }
  `;

StyledTextInput.defaultProps = { theme: Base };

export default styled(StyledTextInput)(getDefaultStyles);
