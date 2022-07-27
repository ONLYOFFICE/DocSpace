import styled, { css } from "styled-components";
import { StyledFilterBlockItemTag } from "@appserver/common/components/FilterInput/sub-components/StyledFilterBlock";

const getDefaultStyles = ({ currentColorScheme, isSelected }) => css`
  background: ${isSelected && currentColorScheme.accentColor};
  border-color: ${isSelected && currentColorScheme.accentColor};
  &:hover {
    background: ${isSelected && currentColorScheme.accentColor};
    border-color: ${isSelected && currentColorScheme.accentColor};
  }
`;

export default styled(StyledFilterBlockItemTag)(getDefaultStyles);
