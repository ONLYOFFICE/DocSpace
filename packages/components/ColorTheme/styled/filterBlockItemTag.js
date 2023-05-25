import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import StyledFilterBlockItemTag from "./sub-components/StyledFilterBlockItemTag";

const getDefaultStyles = ({ $currentColorScheme, isSelected, theme }) =>
  $currentColorScheme &&
  isSelected &&
  css`
    background: ${$currentColorScheme.main.accent};
    border-color: ${$currentColorScheme.main.accent};

    .filter-text {
      color: ${$currentColorScheme.textColor};
    }

    &:hover {
      background: ${$currentColorScheme.main.accent};
      border-color: ${$currentColorScheme.main.accent};
    }
  `;

StyledFilterBlockItemTag.defaultProps = {
  theme: Base,
};

export default styled(StyledFilterBlockItemTag)(getDefaultStyles);
