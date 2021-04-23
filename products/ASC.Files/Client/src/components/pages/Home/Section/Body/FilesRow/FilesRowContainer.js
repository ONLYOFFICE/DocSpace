import React from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import SimpleFilesRow from "./SimpleFilesRow";

const FilesRowContainer = ({ filesList }) => {
  return (
    <Consumer>
      {(context) => (
        <RowContainer
          className="files-row-container"
          draggable
          useReactWindow={false}
        >
          {filesList.map((item, index) => (
            <SimpleFilesRow
              key={`${item.id}_${index}`}
              item={item}
              sectionWidth={context.sectionWidth}
            />
          ))}
        </RowContainer>
      )}
    </Consumer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList } = filesStore;

  return {
    filesList,
  };
})(observer(FilesRowContainer));
