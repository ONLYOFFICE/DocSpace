import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";
import TileContent from "./sub-components/TileContent";
import { tablet, desktop } from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

const FlexBoxStyles = css`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
`;

const checkedStyle = css`
  background: ${(props) =>
    props.theme.filesSection.tilesView.tile.checkedColor} !important;
`;

const StyledTile = styled.div`
  box-sizing: border-box;
  width: 100%;
  border: ${(props) => props.theme.filesSection.tilesView.tile.border};
  border-radius: 6px;
  ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) => props.isSelected && checkedStyle}

  &:before,
  &:after {
    ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"};
  }

  &:before,
  &:after {
    ${(props) => props.isSelected && checkedStyle};
  }

  .file-icon {
    display: flex;
    flex: 0 0 auto;
    user-select: none;
  }

  .file-icon_container {
    width: 32px;
    height: 32px;
    margin-left: 16px;
    margin-right: 8px;
  }
`;

const StyledFileTileTop = styled.div`
  ${FlexBoxStyles};
  background: ${(props) =>
    props.theme.filesSection.tilesView.tile.backgroundColorTop};
  justify-content: space-between;
  align-items: baseline;
  height: 156px;
  overflow: hidden;
  position: relative;
  border-radius: 6px 6px 0 0;

  .thumbnail-image-link {
    margin: 0 auto;

    .thumbnail-image {
      pointer-events: none;
      position: relative;
      height: 100%;
      width: 100%;
      object-fit: cover;
      border-radius: 6px 6px 0 0;
      z-index: 0;
    }
  }

  .temporary-icon > .injected-svg {
    position: absolute;
    width: 100%;
    bottom: 16px;
  }
`;

const StyledFileTileBottom = styled.div`
  ${FlexBoxStyles};

  ${(props) => !props.isEdit && props.isSelected && checkedStyle}

  border-top: 1px solid transparent;
  ${(props) =>
    props.isSelected &&
    css`
      border-top: ${(props) => props.theme.filesSection.tilesView.tile.border};
      border-radius: 0 0 6px 6px;
    `}

  padding: 9px 0;
  height: 62px;
  box-sizing: border-box;

  .tile-file-loader {
    padding-top: 4px;
    padding-left: 3px;
  }
`;

const StyledContent = styled.div`
  display: flex;
  flex-basis: 100%;

  a {
    display: block;
    display: -webkit-box;
    max-width: 400px;
    height: auto;
    margin: 0 auto;
    line-height: 19px;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
    word-break: break-word;
  }

  @media (max-width: 1024px) {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const StyledElement = styled.div`
  flex: 0 0 auto;
  display: flex;
  margin-right: 4px;
  user-select: none;
  margin-top: 3px;

  height: 32px;
  width: 32px;
`;

const StyledOptionButton = styled.div`
  display: block;

  .expandButton > div:first-child {
    padding: 8px 21px 8px 12px;
  }
`;

StyledOptionButton.defaultProps = { theme: Base };

const SimpleFilesTileContent = styled(TileContent)`
  .row-main-container {
    height: auto;
    max-width: 100%;
    align-self: flex-end;
  }

  .main-icons {
    align-self: flex-end;
  }

  .badge {
    margin-right: 8px;
    cursor: pointer;
    height: 16px;
    width: 16px;
  }

  .new-items {
    position: absolute;
    right: 29px;
    top: 19px;
  }

  .badges {
    display: flex;
    align-items: center;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  .favorite,
  .can-convert,
  .edit {
    svg:not(:root) {
      width: 14px;
      height: 14px;
    }
  }

  @media (max-width: 1024px) {
    display: inline-flex;
    height: auto;

    & > div {
      margin-top: 0;
    }
  }
`;

const paddingCss = css`
  @media ${desktop} {
    padding-right: 3px;
  }
`;

const StyledGridWrapper = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  width: 100%;
  margin-bottom: ${(props) => (props.isFolders ? "23px" : 0)};
  box-sizing: border-box;
  ${paddingCss};

  grid-gap: 14px 16px;

  @media ${tablet} {
    grid-gap: 14px;
  }
`;

const StyledTileContainer = styled.div`
  position: relative;
  height: 100%;

  .tile-item-wrapper {
    position: relative;
    width: 100%;

    &.file {
      padding: 0;
    }
    &.folder {
      padding: 0;
    }
  }

  .tile-items-heading {
    margin: 0;
    margin-bottom: 15px;

    display: flex;
    align-items: center;
    justify-content: space-between;

    div {
      cursor: pointer !important;

      .sort-combo-box {
        margin-right: 3px;
        .dropdown-container {
          top: 104%;
          bottom: auto;
          min-width: 200px;
          margin-top: 3px;

          .option-item {
            display: flex;
            align-items: center;
            justify-content: space-between;

            min-width: 200px;

            svg {
              width: 16px;
              height: 16px;
            }

            .option-item__icon {
              display: none;
              cursor: pointer;
              ${(props) =>
                props.isDesc &&
                css`
                  transform: rotate(180deg);
                `}

              path {
                fill: ${(props) => props.theme.filterInput.sort.sortFill};
              }
            }

            :hover {
              .option-item__icon {
                display: flex;
              }
            }
          }

          .selected-option-item {
            background: ${(props) =>
              props.theme.filterInput.sort.hoverBackground};
            cursor: auto;

            .selected-option-item__icon {
              display: flex;
            }
          }
        }

        .optionalBlock {
          display: flex;
          flex-direction: row;
          align-items: center;

          font-size: 12px;
          font-weight: 600;

          color: ${(props) => props.theme.filterInput.sort.tileSortColor};

          .sort-icon {
            margin-right: 8px;
            svg {
              path {
                fill: ${(props) => props.theme.filterInput.sort.tileSortFill};
              }
            }
          }
        }

        .combo-buttons_arrow-icon {
          display: none;
        }
      }
    }
  }

  @media ${tablet} {
    margin-right: -3px;
  }

  ${isMobile &&
  css`
    padding-top: 24px;
  `}
`;

StyledTileContainer.defaultProps = { theme: Base };

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0;
  font-family: "Open Sans";
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const StyledTileContent = styled.div`
  width: 100%;
  display: inline-flex;
`;

const MainContainerWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;
`;

const MainContainer = styled.div`
  height: 20px;

  @media (max-width: 1024px) {
    ${truncateCss};
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

export {
  StyledTile,
  StyledFileTileTop,
  StyledFileTileBottom,
  StyledContent,
  StyledElement,
  StyledOptionButton,
  SimpleFilesTileContent,
  StyledGridWrapper,
  StyledTileContainer,
  StyledTileContent,
  MainContainerWrapper,
  MainContainer,
  StyledCard,
  StyledItem,
};
