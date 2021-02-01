import React from "react";
import { CustomScrollbarsVirtualList } from "@appserver/components";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import { connect } from "react-redux";
import { getUploadDataFiles } from "../../../store/files/selectors";
import RowWrapper from "./RowWrapper";

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

const mapStateToProps = (state) => {
  return {
    uploadDataFiles: getUploadDataFiles(state),
  };
};

export default connect(mapStateToProps)(FileList);
