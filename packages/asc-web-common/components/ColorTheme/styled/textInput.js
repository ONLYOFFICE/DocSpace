import styled, { css } from "styled-components";
import StyledTextInput from "@appserver/components/text-input/styled-text-input";
import Base from "@appserver/components/themes/base";

const getDefaultStyles = ({
  currentColorScheme,
  hasError,
  hasWarning,
  isDisabled,
  theme,
}) => css`
  :focus {
    border-color: ${(hasError && theme.input.focusErrorBorderColor) ||
    (hasWarning && theme.input.focusWarningBorderColor) ||
    (isDisabled && theme.input.focusDisabledBorderColor) ||
    (theme.isBase === true
      ? currentColorScheme.accentColor
      : theme.input.focusBorderColor)};
  }
`;

StyledTextInput.defaultProps = { theme: Base };

export default styled(StyledTextInput)(getDefaultStyles);
