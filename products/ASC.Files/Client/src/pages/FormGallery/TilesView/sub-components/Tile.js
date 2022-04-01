import ContextMenuButton from "@appserver/components/context-menu-button";
import PropTypes from "prop-types";
import React from "react";
import ContextMenu from "@appserver/components/context-menu";
import { isDesktop } from "react-device-detect";
import Link from "@appserver/components/link";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import {
  StyledTile,
  StyledFileTileTop,
  StyledFileTileBottom,
  StyledContent,
  StyledElement,
  StyledOptionButton,
} from "../StyledTileView";

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
    const { thumbnailClick, item, temporaryIcon } = this.props;

    //const src = item.file_image;
    const src = temporaryIcon;
    //const svgLoader = () => <div style={{ width: "96px" }} />;

    return (
      <ReactSVG
        className="temporary-icon"
        src={src} /* loading={svgLoader} */
      />
    );

    return (
      <Link type="page" onClick={thumbnailClick}>
        <img
          src={src}
          //className="thumbnail-image"
          className="temporary-icon"
          alt="Thumbnail-img"
          onError={this.onError}
        />
      </Link>
    );
  };

  onFileIconClick = () => {
    if (isDesktop) return;

    const { onSelect, item } = this.props;
    onSelect && onSelect(true, item);
  };

  getContextModel = () => {
    return [
      {
        key: "create",
        label: this.props.t("Common:Create"),
        onClick: this.onCreateForm,
      },
      {
        key: "template-info",
        label: this.props.t("TemplateInfo"),
        onClick: this.onShowTemplateInfo,
      },
    ];
  };

  onCreateForm = () => {
    console.log("onCreateForm");
  };

  onShowTemplateInfo = () => {
    console.log("onShowTemplateInfo");
  };

  getOptions = () => ["create", "template-info"];

  render() {
    const {
      children,
      contextButtonSpacerWidth,
      element,
      tileContextClick,
      isActive,

      title,
      showHotkeyBorder,
    } = this.props;

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
        isActive={isActive}
        isDesktop={isDesktop}
        showHotkeyBorder={showHotkeyBorder}
      >
        <StyledFileTileTop isActive={isActive}>{icon}</StyledFileTileTop>

        <StyledFileTileBottom isActive={isActive}>
          <div className="file-icon_container">
            <div className="file-icon" onClick={this.onFileIconClick}>
              {element}
            </div>
          </div>

          <StyledContent>{children}</StyledContent>
          <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
            <ContextMenuButton
              className="expandButton"
              directionX="right"
              getData={this.getOptions}
              isNew={true}
              onClick={onContextMenu}
              title={title}
            />

            <ContextMenu
              getContextModel={this.getContextModel}
              ref={this.cm}
              withBackdrop={true}
            />
          </StyledOptionButton>
        </StyledFileTileBottom>
      </StyledTile>
    );
  }
}

Tile.propTypes = {
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
  onSelect: PropTypes.func,
  tileContextClick: PropTypes.func,
};

Tile.defaultProps = {
  contextButtonSpacerWidth: "32px",
  item: {},
};

export default withTranslation(["FormGallery", "Common"])(Tile);
