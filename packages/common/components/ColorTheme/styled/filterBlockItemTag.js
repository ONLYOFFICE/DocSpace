import styled, { css } from "styled-components";
import { StyledFilterBlockItemTag } from "@docspace/common/components/FilterInput/sub-components/StyledFilterBlock";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isSelected, theme }) =>
  $currentColorScheme &&
  isSelected &&
  css`
    background: ${theme.isBase && $currentColorScheme.accentColor};
    border-color: ${theme.isBase && $currentColorScheme.accentColor};

    p {
      color: ${$currentColorScheme.id > 7 && $currentColorScheme.textColor};
    }
    &:hover {
      background: ${theme.isBase && $currentColorScheme.accentColor};
      border-color: ${theme.isBase && $currentColorScheme.accentColor};
    }
  `;

StyledFilterBlockItemTag.defaultProps = {
  theme: Base,
};

export default styled(StyledFilterBlockItemTag)(getDefaultStyles);
