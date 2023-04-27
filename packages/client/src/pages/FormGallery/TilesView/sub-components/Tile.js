import React from "react";
import { inject, observer } from "mobx-react";
import ContextMenuButton from "@docspace/components/context-menu-button";
import PropTypes from "prop-types";
import ContextMenu from "@docspace/components/context-menu";
import Link from "@docspace/components/link";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import FilesFilter from "@docspace/common/api/files/filter";
import { useParams, useNavigate } from "react-router-dom";

import {
  StyledTile,
  StyledFileTileTop,
  StyledFileTileBottom,
  StyledContent,
  StyledOptionButton,
} from "../StyledTileView";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";

const Tile = (props) => {
  const [errorLoadSrc, setErrorLoadSrc] = React.useState(false);

  const cm = React.useRef();
  const tile = React.useRef();

  const {
    t,
    thumbnailClick,
    item,

    setIsInfoPanelVisible,
    categoryType,
    isInfoPanelVisible,
    setGallerySelected,
    children,
    contextButtonSpacerWidth,
    tileContextClick,
    isActive,
    isSelected,
    title,
    showHotkeyBorder,
    getIcon,
  } = props;

  const params = useParams();
  const navigate = useNavigate();

  const onError = () => {
    setErrorLoadSrc(true);
  };

  const getIconFile = () => {
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
          onError={onError}
        />
      </Link>
    ) : (
      <ReactSVG className="temporary-icon" src={src} loading={svgLoader} />
    );
  };

  const getContextModel = () => {
    return [
      {
        key: "create",
        label: t("Common:Create"),
        onClick: onCreateForm,
      },
      {
        key: "template-info",
        label: t("TemplateInfo"),
        onClick: onShowTemplateInfo,
      },
    ];
  };

  const onCreateForm = () => {
    const filter = FilesFilter.getDefault();

    filter.folder = params.folderId;

    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(categoryType, filter.folder);

    const pathname = `${url}?${filterParamsStr}`;

    setIsInfoPanelVisible(false);

    navigate(
      combineUrl(window.DocSpaceConfig?.proxy?.url, config.homepage, pathname)
    );
  };

  const onShowTemplateInfo = () => {
    onSelectForm();
    if (!isInfoPanelVisible) setIsInfoPanelVisible(true);
  };

  const getOptions = () => ["create", "template-info"];

  const onSelectForm = () => {
    setGallerySelected(item);
  };

  const src = getIcon(32, ".docxf");
  const element = <img className="react-svg-icon" src={src} />;

  const onContextMenu = (e) => {
    tileContextClick && tileContextClick();
    if (!cm.current.menuRef.current) {
      tile.current.click(e); //TODO: need fix context menu to global
    }
    cm.current.show(e);
  };

  const icon = getIconFile();

  //TODO: OFORM isActive

  return (
    <StyledTile
      ref={tile}
      isSelected={isSelected}
      onContextMenu={onContextMenu}
      isActive={isActive}
      showHotkeyBorder={showHotkeyBorder}
      onDoubleClick={onCreateForm}
      onClick={onSelectForm}
      className="files-item"
    >
      <StyledFileTileTop isActive={isActive}>{icon}</StyledFileTileTop>

      <StyledFileTileBottom isSelected={isSelected} isActive={isActive}>
        <div className="file-icon_container">
          <div className="file-icon">{element}</div>
        </div>

        <StyledContent>{children}</StyledContent>
        <StyledOptionButton spacerWidth={contextButtonSpacerWidth}>
          <ContextMenuButton
            className="expandButton"
            directionX="right"
            getData={getOptions}
            displayType="toggle"
            onClick={onContextMenu}
            title={title}
          />

          <ContextMenu
            getContextModel={getContextModel}
            ref={cm}
            withBackdrop={true}
          />
        </StyledOptionButton>
      </StyledFileTileBottom>
    </StyledTile>
  );
};

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
)(withTranslation(["FormGallery", "Common"])(observer(Tile)));
