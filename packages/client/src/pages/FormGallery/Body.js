﻿import EmptyScreenFormGalleryReactSvgUrl from "PUBLIC_DIR/images/empty_screen_form-gallery.react.svg?url";
import React, { useEffect } from "react";
import { observer, inject } from "mobx-react";
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import { withTranslation } from "react-i18next";
import TileContainer from "./TilesView/sub-components/TileContainer";
import FileTile from "./TilesView/FileTile";
import Loaders from "@docspace/common/components/Loaders";
import SubmitToGalleryTile from "./TilesView/sub-components/SubmitToGalleryTile";

const SectionBodyContent = ({
  t,
  tReady,
  oformFiles,
  hasGalleryFiles,
  setGallerySelected,
  submitToGalleryTileIsVisible,
  canSubmitToFormGallery,
}) => {
  const onMouseDown = (e) => {
    if (
      e.target.closest(".scroll-body") &&
      !e.target.closest(".files-item") &&
      !e.target.closest(".not-selectable") &&
      !e.target.closest(".info-panel")
    ) {
      setGallerySelected(null);
    }
  };

  useEffect(() => {
    window.addEventListener("mousedown", onMouseDown);
    setGallerySelected(null);

    return () => {
      window.removeEventListener("mousedown", onMouseDown);
    };
  }, [onMouseDown]);

  return !tReady || !oformFiles ? (
    <Loaders.Tiles foldersCount={0} withTitle={false} />
  ) : !hasGalleryFiles ? (
    <EmptyScreenContainer
      imageSrc={EmptyScreenFormGalleryReactSvgUrl}
      imageAlt="Empty Screen Gallery image"
      headerText={t("GalleryEmptyScreenHeader")}
      descriptionText={t("EmptyScreenDescription")}
    />
  ) : (
    <TileContainer className="tile-container">
      {submitToGalleryTileIsVisible && canSubmitToFormGallery() && (
        <SubmitToGalleryTile />
      )}
      {oformFiles.map((item, index) => (
        <FileTile key={`${item.id}_${index}`} item={item} />
      ))}
    </TileContainer>
  );
};

export default inject(({ accessRightsStore, oformsStore }) => ({
  oformFiles: oformsStore.oformFiles,
  hasGalleryFiles: oformsStore.hasGalleryFiles,
  setGallerySelected: oformsStore.setGallerySelected,
  submitToGalleryTileIsVisible: oformsStore.submitToGalleryTileIsVisible,
  canSubmitToFormGallery: accessRightsStore.canSubmitToFormGallery,
}))(withTranslation("FormGallery")(observer(SectionBodyContent)));
