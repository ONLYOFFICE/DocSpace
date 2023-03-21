import styled, { css } from "styled-components";
import { StyledScrollbar } from "@docspace/components/textarea/styled-textarea";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, hasError, theme }) =>
  $currentColorScheme &&
  css`
    :focus-within {
      border-color: ${hasError
        ? theme?.textArea.focusErrorBorderColor
        : theme.textArea.focusBorderColor};
    }
  `;

StyledScrollbar.defaultProps = {
  theme: Base,
};

export default styled(StyledScrollbar)(getDefaultStyles);
