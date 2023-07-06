import styled, { css } from "styled-components";

import ArrowRightSvg from "PUBLIC_DIR/images/arrow.right.react.svg";

import Text from "../../../text";
import { Base } from "../../../themes";

const StyledBreadCrumbs = styled.div<{
  itemsCount: number;
  gridTemplateColumns: string;
}>`
  width: 100%;
  height: 38px;

  padding: 0 16px 16px 16px;

  box-sizing: border-box;

  display: grid;

  grid-template-columns: ${(props) => props.gridTemplateColumns};

  grid-column-gap: 8px;

  align-items: center;

  .context-menu-button {
    transform: rotate(90deg);
  }
`;

const StyledItemText = styled(Text)<{ isCurrent: boolean; isLoading: boolean }>`
  ${(props) =>
    !props.isCurrent &&
    css`
      color: ${props.theme.selector.breadCrumbs.prevItemColor};

      ${!props.isLoading && `cursor: pointer`};
    `}
`;

StyledItemText.defaultProps = { theme: Base };

const StyledArrowRightSvg = styled(ArrowRightSvg)`
  path {
    fill: ${(props) => props.theme.selector.breadCrumbs.arrowRightColor};
  }
`;

StyledArrowRightSvg.defaultProps = { theme: Base };

export { StyledBreadCrumbs, StyledItemText, StyledArrowRightSvg };
