import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import SimpleFilesRow from "./SimpleFilesRow";

const FilesRowContainer = ({ filesList, sectionWidth, viewAs, setViewAs }) => {
  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <RowContainer
      className="files-row-container"
      draggable
      useReactWindow={false}
    >
      {filesList.map((item, index) => (
        <SimpleFilesRow
          key={`${item.id}_${index}`}
          item={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </RowContainer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList, viewAs, setViewAs } = filesStore;

  return {
    filesList,
    viewAs,
    setViewAs,
  };
})(observer(FilesRowContainer));
