import React from "react";
import Loaders from "..";
const RowsLoader = (props) => {
  return (
    <div>
      <Loaders.Row {...props} />
      <Loaders.Row {...props} />
      <Loaders.Row {...props} />
      <Loaders.Row {...props} />
      <Loaders.Row {...props} />
      <Loaders.Row {...props} />
    </div>
  );
};

export default RowsLoader;
