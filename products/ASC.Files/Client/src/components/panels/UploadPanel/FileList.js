import React, { useState, useCallback, useRef } from "react";
import CustomScrollbars from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import AutoSizer from "react-virtualized-auto-sizer";
import { VariableSizeList as List } from "react-window";
import { inject, observer } from "mobx-react";
import FileRow from "./FileRow";

const CustomScrollbarsVirtualList = React.forwardRef((props, ref) => (
  <CustomScrollbars stype="mediumBlack" {...props} forwardedRef={ref} />
));
const FileList = ({ uploadDataFiles }) => {
  const [rowSizes, setRowSizes] = useState({});
  const listRef = useRef(null);
  const onUpdateHeight = (i, showInput) => {
    if (listRef.current) {
      listRef.current.resetAfterIndex(i);
    }
    setRowSizes((prevState) => ({ ...prevState, [i]: showInput ? 88 : 40 }));
  };

  const getSize = (i) => {
    console.log("getSize", i, rowSizes[i]);
    return rowSizes[i] ? rowSizes[i] : 40;
  };

  const renderRow = useCallback(({ data, index, style }) => {
    const item = data[index];
    console.log("rowSizes", rowSizes, index);
    return (
      <div style={style}>
        <FileRow item={item} updateRowsHeight={onUpdateHeight} index={index} />
      </div>
    );
  }, []);

  const renderList = ({ height, width }) => {
    return (
      <>
        <List
          ref={listRef}
          className="List"
          height={height}
          width={width}
          itemSize={getSize}
          itemCount={uploadDataFiles.length}
          itemData={uploadDataFiles}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {renderRow}
        </List>
      </>
    );
  };

  return <AutoSizer>{renderList}</AutoSizer>;
};

export default inject(({ uploadDataStore }) => {
  const { files } = uploadDataStore;

  return {
    uploadDataFiles: files,
  };
})(observer(FileList));
