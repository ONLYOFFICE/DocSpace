import React from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import SimpleFilesRow from "./SimpleFilesRow";

const FilesRowContainer = (props) => {
  return (
    <Consumer>
      {(context) => (
        <RowContainer
          className="files-row-container"
          draggable
          useReactWindow={false}
        >
          {props.filesList.map((item) => {
            return (
              <SimpleFilesRow
                key={item.id}
                item={item}
                sectionWidth={context.sectionWidth}
              />
            );
          })}
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
