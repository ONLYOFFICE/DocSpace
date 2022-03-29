import Checkbox from "@appserver/components/checkbox";
import ContextMenuButton from "@appserver/components/context-menu-button";
import PropTypes from "prop-types";
import React from "react";
import { ReactSVG } from "react-svg";
import styled, { css } from "styled-components";
import ContextMenu from "@appserver/components/context-menu";
import { tablet } from "@appserver/components/utils/device";
import { isDesktop } from "react-device-detect";

import Link from "@appserver/components/link";
import Loader from "@appserver/components/loader";
import { Base } from "@appserver/components/themes";

const svgLoader = () => <div style={{ width: "96px" }} />;

const FlexBoxStyles = css`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
`;

const FolderStyles = css`
  height: 64px;
`;

const FileStyles = css`
  height: 220px;
`;

const checkedStyle = css`
  background: ${(props) =>
    props.theme.filesSection.tilesView.tile.checkedColor} !important;
`;

const bottomFileBorder = css`
  border-top: ${(props) => props.theme.filesSection.tilesView.tile.border};
  border-radius: 0 0 6px 6px;
`;

const StyledTile = styled.div`
  cursor: ${(props) => (!props.isRecycleBin ? "pointer" : "default")};
  ${(props) =>
    props.inProgress &&
    css`
      pointer-events: none;
      /* cursor: wait; */
    `}
  box-sizing: border-box;
  width: 100%;
  border: ${(props) => props.theme.filesSection.tilesView.tile.border};
  border-radius: 6px;
  ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"};
  ${(props) => props.isFolder && "border-top-left-radius: 0px;"}
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) => props.isFolder && FlexBoxStyles}
  ${(props) => (props.isFolder ? FolderStyles : FileStyles)}
  ${(props) =>
    !props.isEdit &&
    props.isFolder &&
    (props.checked || props.isActive) &&
    checkedStyle}


  &:before, 
  &:after {
    ${(props) => props.isFolder && props.dragging && draggingStyle};
    ${(props) => props.showHotkeyBorder && "border-color: #2DA7DB"};
  }

  &:before,
  &:after {
    ${(props) => (props.checked || props.isActive) && checkedStyle};
  }

  &:hover:before,
  &:hover:after {
    ${(props) => props.isFolder && props.dragging && draggingHoverStyle};
  }

  .checkbox {
    display: flex;
    opacity: ${(props) => (props.checked ? 1 : 0)};
    flex: 0 0 16px;
    justify-content: center;

    @media ${tablet} {
      opacity: 1;
    }
  }

  .file-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
    flex: 0 0 16px;
    margin-top: 8px;
    margin-left: ${(props) => (props.isFolder ? "8px" : "7px")};
  }

  .file-icon {
    display: ${(props) => (props.checked ? "none" : "flex")};
    flex: 0 0 auto;
    user-select: none;
    margin-top: ${(props) => (props.isFolder ? "0" : "-2px")};
  }

  .file-icon_container {
    width: 32px;
    height: 32px;
    margin-left: ${(props) => (props.isFolder ? "15px" : "16px")};
    margin-right: ${(props) => (props.isFolder ? "7px" : "8px")};
  }

  .tile-folder-loader {
    padding-top: 4px;
  }

  :hover {
    ${(props) =>
      !props.dragging &&
      props.isDesktop &&
      !props.inProgress &&
      css`
        .checkbox {
          opacity: 1;
        }
        .file-checkbox {
          display: flex;
        }
        .file-icon {
          display: none;
        }
      `}
  }
`;

const StyledFileTileTop = styled.div`
  ${FlexBoxStyles};
  background: ${(props) =>
    props.theme.filesSection.tilesView.tile.backgroundColorTop};
  justify-content: space-between;
  align-items: baseline;
  height: 156px;
  position: relative;

  .thumbnail-image {
    pointer-events: none;
    position: absolute;
    height: 100%;
    width: 100%;
    object-fit: cover;
    border-radius: 6px 6px 0 0;
    z-index: 0;
  }

  .temporary-icon > .injected-svg {
    position: absolute;
    width: 100%;
    bottom: 16px;
  }
`;

const StyledFileTileBottom = styled.div`
  ${FlexBoxStyles};
  ${(props) =>
    !props.isEdit && (props.checked || props.isActive) && checkedStyle}

  border-top: 1px solid transparent;
  ${(props) =>
    !props.isEdit && (props.checked || props.isActive) && bottomFileBorder}

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

const badgesPosition = css`
  left: 9px;

  .badges {
    display: grid;
    grid-template-columns: repeat(3, fit-content(50px));
    grid-template-rows: 32px;
    grid-gap: 7px;

    .badge-new-version {
      order: 1;
    }

    .badge-version-current {
      order: 2;
    }

    .is-editing,
    .can-convert {
      order: 3;
    }
  }
`;

const quickButtonsPosition = css`
  right: 9px;

  .badges {
    display: grid;
    grid-template-columns: 32px;
    grid-template-rows: repeat(3, 32px);
    grid-gap: 7px;
  }
`;

const StyledIcons = styled.div`
  position: absolute;
  top: 8px;

  ${(props) => props.isBadges && badgesPosition}
  ${(props) => props.isQuickButtons && quickButtonsPosition}
  
  .badge {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 8px;
    background: ${(props) =>
      props.theme.filesSection.tilesView.tile.backgroundColor};
    border-radius: 4px;
    box-shadow: 0px 2px 4px rgba(4, 15, 27, 0.16);
  }
`;

StyledIcons.defaultProps = { theme: Base };

class Tile extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      errorLoadSrc: false,
    };

    this.cm = React.createRef();
    this.tile = React.createRef();
  }

  onError = () => {
    this.setState({
      errorLoadSrc: true,
    });
  };

  getIconFile = () => {
    const { temporaryIcon, thumbnailClick, thumbnail } = this.props;

    const icon =
      thumbnail && !this.state.errorLoadSrc ? thumbnail : temporaryIcon;

    return (
      <Link type="page" onClick={thumbnailClick}>
        {thumbnail && !this.state.errorLoadSrc ? (
          <img
            src={thumbnail}
            className="thumbnail-image"
            alt="Thumbnail-img"
            onError={this.onError}
          />
        ) : (
          <ReactSVG className="temporary-icon" src={icon} loading={svgLoader} />
        )}
      </Link>
    );
  };

  changeCheckbox = (e) => {
    const { onSelect, item } = this.props;
    onSelect && onSelect(e.target.checked, item);
  };

  onFileIconClick = () => {
    if (isDesktop) return;

    const { onSelect, item } = this.props;
    onSelect && onSelect(true, item);
  };

  render() {
    const {
      checked,
      children,
      contextButtonSpacerWidth,
      contextOptions,
      element,
      indeterminate,
      tileContextClick,
      dragging,
      isRecycleBin,
      item,
      isActive,
      inProgress,
      isEdit,
      contentElement,
      title,
      getContextModel,
      showHotkeyBorder,
    } = this.props;
    const { isFolder, id, fileExst } = item;

    const renderElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "element"
    );

    const renderContentElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "contentElement"
    );

    const renderContext =
      Object.prototype.hasOwnProperty.call(item, "contextOptions") &&
      contextOptions.length > 0;

    const getOptions = () => {
      tileContextClick && tileContextClick();
      return contextOptions;
    };

    const onContextMenu = (e) => {
      tileContextClick && tileContextClick();
      if (!this.cm.current.menuRef.current) {
        this.tile.current.click(e); //TODO: need fix context menu to global
      }
      this.cm.current.show(e);
    };

    const icon = this.getIconFile();
    const [FilesTileContent, badges] = children;
    const quickButtons = contentElement;

    const contextMenuHeader = {
      icon: children[0].props.item.icon,
      title: children[0].props.item.title,
    };

    return (
      <StyledTile
        ref={this.tile}
        {...this.props}
        onContextMenu={onContextMenu}
        dragging={dragging && isFolder}
        isFolder={(isFolder && !fileExst) || (!fileExst && id === -1)}
        isRecycleBin={isRecycleBin}
        checked={checked}
        isActive={isActive}
        inProgress={inProgress}
        isDesktop={isDesktop}
        showHotkeyBorder={showHotkeyBorder}
      >
        {isFolder || (!fileExst && id === -1) ? (
          <>
            {renderElement && !(!fileExst && id === -1) && !isEdit && (
              <>
                {!inProgress ? (
                  <div className="file-icon_container">
                    <StyledElement
                      className="file-icon"
                      onClick={this.onFileIconClick}
                    >
                      {element}
                    </StyledElement>

                    <Checkbox
                      className="checkbox file-checkbox"
                      isChecked={checked}
                      isIndeterminate={indeterminate}
                      onChange={this.changeCheckbox}
                    />
                  </div>
                ) : (
                  <Loader
                    className="tile-folder-loader"
                    type="oval"
                    size="16px"
                  />
                )}
              </>
            )}
            <StyledContent
              isFolder={(isFolder && !fileExst) || (!fileExst && id === -1)}
            >
              {FilesTileContent}
            </StyledContent>
            <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
              {renderContext ? (
                <ContextMenuButton
                  className="expandButton"
                  directionX="right"
                  getData={getOptions}
                  isNew={true}
                  onClick={onContextMenu}
                  title={title}
                />
              ) : (
                <div className="expandButton" />
              )}
              <ContextMenu
                getContextModel={getContextModel}
                ref={this.cm}
                header={contextMenuHeader}
              />
            </StyledOptionButton>
          </>
        ) : (
          <>
            <StyledFileTileTop checked={checked} isActive={isActive}>
              {icon}
            </StyledFileTileTop>

            <StyledIcons isBadges>{badges}</StyledIcons>

            {renderContentElement && (
              <StyledIcons isQuickButtons>{quickButtons}</StyledIcons>
            )}

            <StyledFileTileBottom
              checked={checked}
              isActive={isActive}
              isEdit={isEdit}
            >
              {id !== -1 && !isEdit && (
                <>
                  {!inProgress ? (
                    <div className="file-icon_container">
                      <div className="file-icon" onClick={this.onFileIconClick}>
                        {element}
                      </div>
                      <Checkbox
                        className="file-checkbox"
                        isChecked={checked}
                        isIndeterminate={indeterminate}
                        onChange={this.changeCheckbox}
                      />
                    </div>
                  ) : (
                    <Loader
                      className="tile-file-loader"
                      type="oval"
                      size="16px"
                    />
                  )}
                </>
              )}
              <StyledContent
                isFolder={(isFolder && !fileExst) || (!fileExst && id === -1)}
              >
                {FilesTileContent}
              </StyledContent>
              <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
                {renderContext ? (
                  <ContextMenuButton
                    className="expandButton"
                    directionX="right"
                    getData={getOptions}
                    isNew={true}
                    onClick={onContextMenu}
                    title={title}
                  />
                ) : (
                  <div className="expandButton" />
                )}
                <ContextMenu
                  getContextModel={getContextModel}
                  ref={this.cm}
                  header={contextMenuHeader}
                  withBackdrop={true}
                />
              </StyledOptionButton>
            </StyledFileTileBottom>
          </>
        )}
      </StyledTile>
    );
  }
}

Tile.propTypes = {
  checked: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.element),
    PropTypes.element,
  ]),
  className: PropTypes.string,
  contextButtonSpacerWidth: PropTypes.string,
  contextOptions: PropTypes.array,
  data: PropTypes.object,
  element: PropTypes.element,
  id: PropTypes.string,
  indeterminate: PropTypes.bool,
  needForUpdate: PropTypes.func,
  onSelect: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  viewAs: PropTypes.string,
  tileContextClick: PropTypes.func,
  contentElement: PropTypes.element,
};

Tile.defaultProps = {
  contextButtonSpacerWidth: "32px",
  item: {},
};

export default Tile;
