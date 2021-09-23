import Checkbox from "@appserver/components/checkbox";
import ContextMenuButton from "@appserver/components/context-menu-button";
import PropTypes from "prop-types";
import React from "react";
import { ReactSVG } from "react-svg";
import styled, { css } from "styled-components";
import ContextMenu from "@appserver/components/context-menu";
import { tablet } from "@appserver/components/utils/device";

import Link from "@appserver/components/link";

const svgLoader = () => <div style={{ width: "96px" }}></div>;

const FlexBoxStyles = css`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;
`;

const FolderStyles = css`
  padding-left: 13px;
  padding-bottom: 2px;
  box-sizing: border-box;
`;

const StyledTile = styled.div`
  cursor: ${(props) => (!props.isRecycleBin ? "pointer" : "default")};
  min-height: 57px;
  width: 100%;
  border: 1px solid #d0d5da;
  border-radius: 3px;
  ${(props) => props.isFolder && "border-top-left-radius: 0px;"}
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) => props.isFolder && FlexBoxStyles}
  ${(props) => props.isFolder && FolderStyles}
    ${(props) =>
    props.isFolder &&
    css`
      &:before {
        content: "";
        position: absolute;
        top: -5px;
        left: 1px;
        border-top: 1px solid #d0d5da;
        border-top-left-radius: 3px;
        border-left: 1px solid #d0d5da;
        width: 38px;
        height: 8px;
        background-color: #fff;
        border-bottom: transparent;

        @media ${tablet} {
          left: 0px;
        }
      }
      &:after {
        content: "";
        position: absolute;
        top: -3.5px;
        left: 36px;
        border-top: 1px solid #d0d5da;
        background-color: #fff;
        width: 9px;
        height: 10px;
        transform: rotateZ(35deg);

        @media ${tablet} {
          left: 35px;
        }
      }
    `}
    ${(props) =>
    props.isFolder &&
    props.dragging &&
    css`
      &:before {
        background-color: #f8f7bf;
      }
      &:after {
        background-color: #f8f7bf;
      }
    `}
    ${(props) =>
    props.isFolder &&
    props.dragging &&
    css`
      &:hover:before {
        background-color: #efefb2;
      }
      &:hover:after {
        background-color: #efefb2;
      }
    `};

  .checkbox {
    opacity: ${(props) => (props.checked ? 1 : 0)};
    flex: 0 0 16px;
    margin-right: 4px;
  }

  .file-checkbox {
    display: ${(props) => (props.checked ? "flex" : "none")};
    flex: 0 0 16px;
    margin-right: 4px;
  }

  .file-icon {
    display: ${(props) => (props.checked ? "none" : "flex")};
    flex: 0 0 auto;
    margin-right: 4px;
    user-select: none;
    margin-top: 3px;

    height: 32px;
    width: 32px;
  }

  :hover {
    .checkbox {
      opacity: 1;
    }
    .file-checkbox {
      display: flex;
    }
    .file-icon {
      display: none;
    }
  }
`;

const StyledFileTileTop = styled.div`
  ${FlexBoxStyles}
  justify-content: space-between;
  align-items: baseline;
  background-color: #f8f9f9;
  padding: 13px;
  height: 157px;
  position: relative;

  .thumbnail-image,
  .temporary-icon > .injected-svg {
    pointer-events: none;
    position: absolute;
    left: 0;
    right: 0;
    bottom: 0;
    margin: auto;
    z-index: 0;
    margin-bottom: 16px;
  }
`;

const StyledFileTileBottom = styled.div`
  ${FlexBoxStyles}
  padding: 9px 10px;
  padding-right: 0;
  min-height: 56px;
  box-sizing: border-box;
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
    padding-top: 8px;
    padding-bottom: 8px;
    padding-left: 12px;
    padding-right: 13px;
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
      >
        {isFolder || (!fileExst && id === -1) ? (
          <>
            {renderCheckbox && (
              <Checkbox
                className="checkbox"
                isChecked={checked}
                isIndeterminate={indeterminate}
                onChange={this.changeCheckbox}
              />
            )}
            {renderElement && !(isFolder || (!fileExst && id === -1)) && (
              <StyledElement>{element}</StyledElement>
            )}
            <StyledContent
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
                <div className="expandButton"> </div>
              )}
              <ContextMenu model={contextOptions} ref={this.cm}></ContextMenu>
            </StyledOptionButton>
          </>
        ) : (
          <>
            <StyledFileTileTop>{icon}</StyledFileTileTop>
            <StyledFileTileBottom>
              <div className="file-icon_container">
                <div className="file-icon">{element}</div>
                <Checkbox
                  className="file-checkbox"
                  isChecked={checked}
                  isIndeterminate={indeterminate}
                  onChange={this.changeCheckbox}
                />
              </div>
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
                  <div className="expandButton"> </div>
                )}
                <ContextMenu model={contextOptions} ref={this.cm}></ContextMenu>
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
