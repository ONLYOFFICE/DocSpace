import React from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import SimpleFilesRow from "./SimpleFilesRow";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";

const FilesRowContainer = (props) => {
  const { isLoaded, isLoading } = props;
  return !isLoaded || (isMobile && isLoading) ? (
    <Loaders.Rows />
  ) : (
    <Consumer>
      {(context) => (
        <RowContainer
          className="files-row-container"
          draggable
          useReactWindow={false}
        >
          {props.filesList.map((item) => (
            <SimpleFilesRow
              key={item.id}
              item={item}
              sectionWidth={context.sectionWidth}
            />
          ))}
        </RowContainer>
      )}
    </Consumer>
  );
};

export default inject(({ auth, filesStore }) => {
  const { filesList, isLoading } = filesStore;

  return {
    filesList,
    isLoading,
    isLoaded: auth.isLoaded,
  };
})(observer(FilesRowContainer));
