import styled, { css } from "styled-components";

import { Base } from "../../../themes";

const selectedCss = css`
  background: ${(props) =>
    props.theme.selector.item.selectedBackground} !important;
`;

const StyledItem = styled.div<{
  isSelected: boolean | undefined;
  isMultiSelect: boolean;
}>`
  display: flex;
  align-items: center;

  padding: 0 16px;

  box-sizing: border-box;

  ${(props) => props.isSelected && !props.isMultiSelect && selectedCss}

  .room-logo,
  .user-avatar {
    min-width: 32px;
  }

  .room-logo {
    height: 32px;

    border-radius: 6px;
  }

  .label {
    width: 100%;
    max-width: 100%;

    line-height: 16px;

    margin-left: 8px;
  }

  .checkbox {
    svg {
      margin-right: 0px;
    }
  }

  @media (hover: hover) {
    &:hover {
      cursor: pointer;
      background: ${(props) => props.theme.selector.item.hoverBackground};
    }
  }
`;

StyledItem.defaultProps = { theme: Base };

export default StyledItem;
