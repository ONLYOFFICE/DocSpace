import React, { useRef } from "react";

import TableContainer from "./TableContainer";

export const Table = (props) => {
  const { sectionWidth } = props;
  const ref = useRef(null);

  return <TableContainer sectionWidth={sectionWidth} forwardedRef={ref} />;
};
