import React from "react";
import { CustomScrollbarsVirtualList } from "asc-web-components";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import RowWrapper from "./RowWrapper";
import { inject, observer } from "mobx-react";

const FileList = ({ uploadDataFiles }) => {
  //console.log("FileList render");

  return (
    <AutoSizer>
      {({ height, width, style }) => (
        <List
          style={style}
          height={height}
          width={width}
          itemData={uploadDataFiles}
          itemCount={uploadDataFiles.length}
          itemSize={46}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {RowWrapper}
        </List>
      )}
    </AutoSizer>
  );
};

export default inject(({ uploadDataStore }) => {
  const { files } = uploadDataStore;

  return {
    uploadDataFiles: files,
  };
})(observer(FileList));
