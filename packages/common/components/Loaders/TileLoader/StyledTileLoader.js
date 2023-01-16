import styled, { css } from "styled-components";
import { smallTablet, tablet, size } from "@docspace/components/utils/device";
import Base from "@docspace/components/themes/base";

const StyledTile = styled.div`
  position: relative;
  display: grid;
  width: 100%;

  @media ${smallTablet} {
    &:nth-of-type(n + 3) {
      display: none;
    }
  }

  @media (min-width: ${size.smallTablet}px) and ${tablet} {
    &:nth-of-type(n + 8) {
      display: none;
    }
  }
`;

StyledTile.defaultProps = { theme: Base };

const StyledMainContent = styled.div`
  height: 156px;

  .main-content {
    box-sizing: border-box;
    border: ${(props) => props.theme.filesSection.tilesView.tile.border};
    border-bottom-style: none;
    border-radius: 6px 6px 0 0;
  }
`;

const StyledBottom = styled.div`
  display: flex;
  align-items: center;
  border:${(props) => props.theme.filesSection.tilesView.tile.border};
  border-radius: 6px;
  padding: 15px;

  .first-content {
    height: 32px;
    width: 32px;
    min-width: 32px;
  }

  .second-content {
    width: 100%;
    margin-left: 8px;
  }

  {${(props) =>
    !props.isFolder &&
    css`
      border-top-style: none;
      border-radius: 0 0 6px 6px;
    `}

  .option-button {
    min-width: 16px;
    margin-left: 8px;
  }
`;

export { StyledTile, StyledBottom, StyledMainContent };
