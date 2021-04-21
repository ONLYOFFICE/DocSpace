import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import { Consumer } from "@appserver/components/utils/context";
import SimpleFilesRow from "./SimpleFilesRow";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";

let loadTimeout = null;

const FilesRowContainer = ({ isLoaded, isLoading, filesList, tReady }) => {
  const [inLoad, setInLoad] = useState(false);

  const cleanTimer = () => {
    loadTimeout && clearTimeout(loadTimeout);
    loadTimeout = null;
  };

  useEffect(() => {
    if (isLoading) {
      cleanTimer();
      loadTimeout = setTimeout(() => {
        console.log("inLoad", true);
        setInLoad(true);
      }, 500);
    } else {
      cleanTimer();
      console.log("inLoad", false);
      setInLoad(false);
    }

    return () => {
      cleanTimer();
    };
  }, [isLoading]);

  return !isLoaded || (isMobile && inLoad) || !tReady ? (
    <Loaders.Rows />
  ) : (
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

export default inject(({ auth, filesStore }) => {
  const { filesList, isLoading } = filesStore;

  return {
    filesList,
    isLoading,
    isLoaded: auth.isLoaded,
  };
})(observer(FilesRowContainer));
