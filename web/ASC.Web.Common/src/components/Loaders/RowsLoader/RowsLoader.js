import React from "react";
import RowLoader from "../RowLoader/index";
const RowsLoader = (props) => {
  return (
    <div>
      <RowLoader {...props} />
      <RowLoader {...props} />
      <RowLoader {...props} />
      <RowLoader {...props} />
      <RowLoader {...props} />
      <RowLoader {...props} />
    </div>
  );
};

export default RowsLoader;
