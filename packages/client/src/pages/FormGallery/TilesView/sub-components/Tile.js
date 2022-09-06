import React from "react";
import { inject, observer } from "mobx-react";
import ContextMenuButton from "@docspace/components/context-menu-button";
import PropTypes from "prop-types";
import ContextMenu from "@docspace/components/context-menu";
import { isDesktop } from "react-device-detect";
import Link from "@docspace/components/link";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import FilesFilter from "@docspace/common/api/files/filter";
import { withRouter } from "react-router-dom";

import {
  StyledTile,
  StyledFileTileTop,
  StyledFileTileBottom,
  StyledContent,
  StyledOptionButton,
} from "../StyledTileView";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";

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
    const { thumbnailClick, item } = this.props;

    // const src =
    //   item.attributes.template_image.data.attributes.formats.small.url;
    const src = item.attributes.card_prewiew.data.attributes.url;
    const svgLoader = () => <div style={{ width: "96px" }} />;

    return src ? (
      <Link
        className="thumbnail-image-link"
        type="page"
        onClick={thumbnailClick}
      >
        <img
          src={src}
          className="thumbnail-image"
          alt="Thumbnail-img"
          onError={this.onError}
        />
      </Link>
    ) : (
      <ReactSVG className="temporary-icon" src={src} loading={svgLoader} />
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
    const { match, history, setIsInfoPanelVisible, categoryType } = this.props;

    const filter = FilesFilter.getDefault();

    filter.folder = match.params.folderId;

    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(categoryType, filter.folder);

    const pathname = `${url}?${filterParamsStr}`;

    setIsInfoPanelVisible(false);

    history.push(
      combineUrl(AppServerConfig.proxyURL, config.homepage, pathname)
    );
  };

  onShowTemplateInfo = () => {
    if (!this.props.isInfoPanelVisible) {
      this.props.setIsInfoPanelVisible(true);
    }
  };

  getOptions = () => ["create", "template-info"];

  onSelectForm = () => {
    this.props.setGallerySelected(this.props.item);
  };

  render() {
    const {
      children,
      contextButtonSpacerWidth,
      tileContextClick,
      isActive,
      isSelected,
      title,
      showHotkeyBorder,
      getIcon,
    } = this.props;

    const src = getIcon(32, ".docxf");
    const element = <img className="react-svg-icon" src={src} />;

    const onContextMenu = (e) => {
      tileContextClick && tileContextClick();
      if (!this.cm.current.menuRef.current) {
        this.tile.current.click(e); //TODO: need fix context menu to global
      }
      this.cm.current.show(e);
    };

    const icon = this.getIconFile();

    //TODO: OFORM isActive

    return (
      <StyledTile
        ref={this.tile}
        isSelected={isSelected}
        onContextMenu={onContextMenu}
        isActive={isActive}
        isDesktop={isDesktop}
        showHotkeyBorder={showHotkeyBorder}
        onDoubleClick={this.onCreateForm}
        onClick={this.onSelectForm}
        className="files-item"
      >
        <StyledFileTileTop isActive={isActive}>{icon}</StyledFileTileTop>

        <StyledFileTileBottom isSelected={isSelected} isActive={isActive}>
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
  id: PropTypes.string,
  onSelect: PropTypes.func,
  tileContextClick: PropTypes.func,
};

Tile.defaultProps = {
  contextButtonSpacerWidth: "32px",
  item: {},
};

export default inject(
  ({ filesStore, settingsStore, auth, oformsStore }, { item }) => {
    const { categoryType } = filesStore;
    const { gallerySelected, setGallerySelected } = oformsStore;
    const { getIcon } = settingsStore;
    const { isVisible, setIsVisible } = auth.infoPanelStore;

    const isSelected = item.id === gallerySelected?.id;

    return {
      isSelected,
      setGallerySelected,
      getIcon,
      setIsInfoPanelVisible: setIsVisible,
      isInfoPanelVisible: isVisible,
      categoryType,
    };
  }
)(withTranslation(["FormGallery", "Common"])(withRouter(observer(Tile))));
