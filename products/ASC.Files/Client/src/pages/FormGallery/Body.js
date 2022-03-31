import React from "react";
import { observer, inject } from "mobx-react";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";
import { withTranslation } from "react-i18next";
import TileContainer from "./TilesView/sub-components/TileContainer";
import FileTile from "./TilesView/FileTile";

const SectionBodyContent = ({ oformFiles, hasOFORMFilesGallery, t }) => {
  //console.log("oformFiles", oformFiles);

  return !hasOFORMFilesGallery ? (
    <EmptyScreenContainer
      imageSrc="images/empty_screen_form-gallery.react.svg"
      imageAlt="Empty Screen Filter image"
      headerText={t("EmptyScreenHeader")}
      descriptionText={t("EmptyScreenDescription")}
    />
  ) : (
    <TileContainer useReactWindow={false} className="tile-container">
      {oformFiles.map((item, index) => (
        <FileTile key={`${item.id}_${index}`} item={item} />
      ))}
    </TileContainer>
  );
};

export default inject(({ filesStore }) => ({
  oformFiles: filesStore.oformFiles,
  hasOFORMFilesGallery: filesStore.hasOFORMFilesGallery,
}))(withTranslation("FormGallery")(observer(SectionBodyContent)));
