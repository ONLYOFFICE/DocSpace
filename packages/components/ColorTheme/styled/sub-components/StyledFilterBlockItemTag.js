import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const selectedItemTag = css`
  background: ${(props) =>
    props.theme.filterInput.filter.selectedItem.background};
  border-color: ${(props) =>
    props.theme.filterInput.filter.selectedItem.border};
`;

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

export default StyledFilterBlockItemTag;
