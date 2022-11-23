import styled, { css } from "styled-components";
import { desktop, tablet } from "@docspace/components/utils/device";

const paddingCss = css`
  @media ${desktop} {
    margin-left: 1px;
    padding-right: 0px;
  }

  @media ${tablet} {
    margin-left: -1px;
  }
`;

const StyledCard = styled.div`
  display: block;
  height: ${({ cardHeight }) => `${cardHeight}px`};
`;

const StyledItem = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  gap: 14px 16px;
  width: 100%;

  @media ${tablet} {
    gap: 14px;
  }

  ${paddingCss};
`;

const StyledHeaderItem = styled.div`
  height: 20px;
  grid-column: -1 / 1;
`;

export { StyledCard, StyledItem, StyledHeaderItem };
