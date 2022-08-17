import styled, { css } from "styled-components";
import { StyledComboButton } from "@docspace/components/combobox/sub-components/styled-combobutton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, isOpen, theme }) =>
  currentColorScheme &&
  css`
    border-color: ${isOpen &&
    (theme.isBase
      ? currentColorScheme.accentColor
      : theme.comboBox.button.openBorderColor)};
  `;

StyledComboButton.defaultProps = { theme: Base };

export default styled(StyledComboButton)(getDefaultStyles);
