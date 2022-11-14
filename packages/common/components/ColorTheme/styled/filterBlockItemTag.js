import styled, { css } from "styled-components";
import { StyledFilterBlockItemTag } from "@docspace/common/components/FilterInput/sub-components/StyledFilterBlock";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, isSelected, theme }) =>
  $currentColorScheme &&
  isSelected &&
  css`
    background: ${$currentColorScheme.main.accent};
    border-color: ${$currentColorScheme.main.accent};

    p {
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
