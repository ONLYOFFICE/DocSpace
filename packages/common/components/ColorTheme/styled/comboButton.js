import styled, { css } from "styled-components";
import { StyledComboButton } from "@docspace/components/combobox/sub-components/styled-combobutton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isOpen, theme }) =>
  $currentColorScheme &&
  css`
    border-color: ${isOpen &&
    (theme.isBase
      ? $currentColorScheme.main.accent
      : theme.comboBox.button.openBorderColor)};

    :focus {
      border-color: ${isOpen &&
      (theme.isBase
        ? $currentColorScheme.main.accent
        : theme.comboBox.button.hoverBorderColor)};
    }
  `;

StyledComboButton.defaultProps = { theme: Base };

export default styled(StyledComboButton)(getDefaultStyles);
