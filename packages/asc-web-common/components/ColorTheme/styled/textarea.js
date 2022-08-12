import styled, { css } from "styled-components";
import { StyledScrollbar } from "@appserver/components/textarea/styled-textarea";
import Base from "@appserver/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, hasError, theme }) => css`
  :focus-within {
    border-color: ${hasError
      ? theme.textArea.focusErrorBorderColor
      : theme.isBase === true
      ? currentColorScheme.accentColor
      : theme.textArea.focusBorderColor};
  }
`;

StyledScrollbar.defaultProps = {
  theme: Base,
};

export default styled(StyledScrollbar)(getDefaultStyles);
