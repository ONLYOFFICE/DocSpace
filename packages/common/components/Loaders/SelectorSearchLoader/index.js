import React from "react";
import PropTypes from "prop-types";

import RectangleLoader from "../RectangleLoader/RectangleLoader";

const SelectorSearchLoader = ({
  id,
  className,
  style,

  ...rest
}) => {
  return (
    <RectangleLoader
      width={"calc(100% - 16px)"}
      height={"32px"}
      style={{ padding: "0 0 0 16px", marginBottom: "8px", ...style }}
      {...rest}
    />
  );
};

export default SelectorSearchLoader;
