import React from "react";
import { inject, observer } from "mobx-react";
import TileContainer from "./sub-components/TileContainer";
import FileTile from "./FileTile";

const FilesTileContainer = ({ filesList, t, sectionWidth }) => {
  return (
    <TileContainer
      className="tile-container"
      draggable
      useReactWindow={false}
      headingFolders={t("Folders")}
      headingFiles={t("Files")}
    >
      {filesList.map((item, index) => (
        <FileTile
          key={`${item.id}_${index}`}
          item={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </TileContainer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList } = filesStore;

  return {
    filesList,
  };
})(observer(FilesTileContainer));
