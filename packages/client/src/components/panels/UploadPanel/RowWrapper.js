import React, { memo } from "react";
import { areEqual } from "react-window";
import FileRow from "./FileRow";

const RowWrapper = memo(({ data, index, style }) => {
  const item = data[index];
  //console.log("RowWrapper render");
  return (
    <div style={style}>
      <FileRow item={item} />
    </div>
  );
}, areEqual);

export default RowWrapper;
