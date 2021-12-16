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

const draggingStyle = css`
  background-color: #f8f7bf;
`;

const draggingHoverStyle = css`
  background-color: #efefb2;
`;

const checkedStyle = css`
  background: #f3f4f4 !important;
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
  border: 1px solid #d0d5da;
  border-radius: 3px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) => props.isFolder && FlexBoxStyles}
  ${(props) => (props.isFolder ? FolderStyles : FileStyles)}
  ${(props) => (props.checked || props.isActive) && checkedStyle}

  &:before, 
  &:after {
    ${(props) => props.isFolder && props.dragging && draggingStyle};
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
    margin-right: 4px;
    justify-content: center;

    @media ${tablet} {
      opacity: 1;
    }
  }

  .file-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
    flex: 0 0 16px;
    margin-top: 8px;
    margin-left: 12px;
  }

  .file-icon {
    display: ${(props) => (props.checked ? "none" : "flex")};
    flex: 0 0 auto;
    margin-left: 5px;
    user-select: none;
    margin-top: ${(props) => (props.isFolder ? "-3px" : "-4px")};

    height: 31px;
    width: 31px;

    img {
      height: 30px;
      width: 30px;
    }
  }

  .file-icon_container {
    width: 32px;
    height: 32px;
    margin-left: 12px;
  }

  .tile-folder-loader {
    padding-top: 4px;
  }

  .styled-content {
    margin-left: ${(props) => (props.isFolder ? "12px" : "14px")};
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
  justify-content: space-between;
  align-items: baseline;
  background-color: #f8f9f9;
  height: 154px;
  position: relative;
  border-bottom: ${(props) =>
    props.checked || props.isActive
      ? "1px solid #D0D5DA"
      : "1px solid transparent"};

  .thumbnail-image,
  .temporary-icon > .injected-svg {
    pointer-events: none;
    position: absolute;
    left: 0;
    right: 0;
    bottom: 0;
    margin: auto;
    z-index: 0;
    width: 190px;
    height: 132px;
  }

  .temporary-icon > .injected-svg {
    margin-bottom: 16px;
  }
`;

const StyledFileTileBottom = styled.div`
  ${FlexBoxStyles};
  padding: 9px 0;
  height: 64px;
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
    font-size: 15px;
    line-height: 19px;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
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
    padding: 8px 13px 8px 12px;
  }
`;

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
    } = this.props;
    const { isFolder, id, fileExst } = item;

    const renderCheckbox = Object.prototype.hasOwnProperty.call(
      this.props,
      "checked"
    );

    const renderElement = Object.prototype.hasOwnProperty.call(
      this.props,
      "element"
    );

    const renderContext =
      Object.prototype.hasOwnProperty.call(this.props, "contextOptions") &&
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
                      className="file-checkbox"
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
              className="styled-content"
              isFolder={(isFolder && !fileExst) || (!fileExst && id === -1)}
            >
              {children}
            </StyledContent>
            <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
              {renderContext ? (
                <ContextMenuButton
                  color="#A3A9AE"
                  hoverColor="#657077"
                  className="expandButton"
                  directionX="right"
                  getData={getOptions}
                  isNew={true}
                  onClick={onContextMenu}
                />
              ) : (
                <div className="expandButton" />
              )}
              <ContextMenu model={contextOptions} ref={this.cm} />
            </StyledOptionButton>
          </>
        ) : (
          <>
            <StyledFileTileTop checked={checked} isActive={isActive}>
              {icon}
            </StyledFileTileTop>
            <StyledFileTileBottom>
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
                className="styled-content"
                isFolder={(isFolder && !fileExst) || (!fileExst && id === -1)}
              >
                {children}
              </StyledContent>
              <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
                {renderContext ? (
                  <ContextMenuButton
                    color="#A3A9AE"
                    hoverColor="#657077"
                    className="expandButton"
                    directionX="right"
                    getData={getOptions}
                    isNew={true}
                    onClick={onContextMenu}
                  />
                ) : (
                  <div className="expandButton" />
                )}
                <ContextMenu model={contextOptions} ref={this.cm} />
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
  children: PropTypes.element,
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
};

Tile.defaultProps = {
  contextButtonSpacerWidth: "32px",
};

export default Tile;
