import styled, { css } from "styled-components";
import { StyledFilterBlockItemTag } from "@docspace/common/components/FilterInput/sub-components/StyledFilterBlock";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, isSelected, theme }) =>
  currentColorScheme &&
  css`
    background: ${isSelected && theme.isBase && currentColorScheme.accentColor};
    border-color: ${isSelected &&
    theme.isBase &&
    currentColorScheme.accentColor};
    &:hover {
      background: ${isSelected &&
      theme.isBase &&
      currentColorScheme.accentColor};
      border-color: ${isSelected &&
      theme.isBase &&
      currentColorScheme.accentColor};
    }
  `;

StyledFilterBlockItemTag.defaultProps = {
  theme: Base,
};

export default styled(StyledFilterBlockItemTag)(getDefaultStyles);
