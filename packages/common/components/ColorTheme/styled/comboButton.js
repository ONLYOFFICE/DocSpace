import styled, { css } from "styled-components";
import { StyledComboButton } from "@docspace/components/combobox/sub-components/styled-combobutton";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isOpen, theme }) =>
  $currentColorScheme &&
  theme.isBase &&
  css`
    border-color: ${isOpen && $currentColorScheme.main.accent};

    :focus {
      border-color: ${isOpen && $currentColorScheme.main.accent};
    }
  `;

export default styled(StyledComboButton)(getDefaultStyles);
