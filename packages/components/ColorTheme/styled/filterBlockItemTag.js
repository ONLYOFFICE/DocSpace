import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledFilterBlockItemTag = styled.div`
  height: 28px;
  max-height: 28px;

  max-width: 100%;

  display: flex;
  flex-direction: row;
  align-items: center;

  border: ${(props) => props.theme.filterInput.filter.border};
  border-radius: 16px;

  box-sizing: border-box;

  padding: 4px 15px;

  cursor: pointer;

  ${(props) => props.isSelected && selectedItemTag}

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

StyledFilterBlockItemTag.defaultProps = { theme: Base };

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
