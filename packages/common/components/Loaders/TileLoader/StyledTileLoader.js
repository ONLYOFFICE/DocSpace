import styled, { css } from "styled-components";
import { smallTablet, tablet, size } from "@docspace/components/utils/device";
import Base from "@docspace/components/themes/base";

const StyledTile = styled.div`
  position: relative;
  display: grid;
  width: 100%;
  height: ${(props) => (props.isFolder ? "32px" : "220px")};

  @media ${smallTablet} {
    &:nth-of-type(n + 3) {
      display: none;
    }

    ${(props) =>
      props.isFolder &&
      css`
        &:nth-of-type(n + 2) {
          display: none;
        }
      `}
  }

  @media (min-width: ${size.smallTablet}px) and ${tablet} {
    &:nth-of-type(n + 7) {
      display: none;
    }
  }
`;

StyledTile.defaultProps = { theme: Base };

const StyledMainContent = styled.div`
  height: 172px;
`;

const StyledBottom = styled.div`
  display: flex;
  align-items: ${(props) => (props.isFolder ? "center" : "stretch")};
  margin-top: ${(props) => (props.isFolder ? 0 : "10px")};
  height: 38px;

  .first-content {
    height: 32px;
    width: 32px;
    min-width: 32px;
  }

  .second-content {
    width: 100%;
    height: ${(props) => (props.isFolder ? "32px" : "16px")};
    margin-left: 8px;
  }

  .files-second-content {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    align-items: flex-end;
    margin-left: 8px;

    .second-content {
      &:last-of-type {
        height: 12px;
        margin-right: auto;
        margin-left: 0;
        width: 50%;
        border-radius: 3px;
      }
    }
  }

  .option-button {
    height: 16px;
    width: 16px;
    min-width: 16px;
    margin-left: 8px;
    border-radius: 6px;
  }
`;

export { StyledTile, StyledBottom, StyledMainContent };
