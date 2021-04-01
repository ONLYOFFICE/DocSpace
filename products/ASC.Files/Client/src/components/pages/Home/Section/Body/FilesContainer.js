import React from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { withTranslation } from "react-i18next";

import { Consumer } from "@appserver/components/utils/context";
import Loaders from "@appserver/common/components/Loaders";
import RowContainer from "@appserver/components/row-container";

import SimpleFilesRow from "./FilesRow/SimpleFilesRow";
import FileTile from "./FilesTile/FileTile";
import TileContainer from "./FilesTile/sub-components/TileContainer";

const FilesContainer = (props) => {
  const { isLoaded, isLoading, filesList, viewAs, t } = props;

  return !isLoaded || (isMobile && isLoading) ? (
    <Loaders.Rows />
  ) : (
    <Consumer>
      {(context) =>
        viewAs === "tile" ? (
          <TileContainer
            className="tileContainer"
            draggable
            useReactWindow={false}
            headingFolders={t("Folders")}
            headingFiles={t("Files")}
          >
            {filesList.map((item) => {
              return <FileTile key={item.id} item={item} />;
            })}
          </TileContainer>
        ) : (
          <RowContainer
            className="files-row-container"
            draggable
            useReactWindow={false}
          >
            {filesList.map((item) => {
              return (
                <SimpleFilesRow
                  key={item.id}
                  item={item}
                  sectionWidth={context.sectionWidth}
                />
              );
            })}
          </RowContainer>
        )
      }
    </Consumer>
  );
};

export default inject(({ auth, initFilesStore, filesStore }) => {
  const { filesList } = filesStore;
  const { isLoading } = initFilesStore;
  const { isLoaded } = auth;

  return {
    filesList,
    isLoading,
    isLoaded,
  };
})(withTranslation("Home")(observer(FilesContainer)));
