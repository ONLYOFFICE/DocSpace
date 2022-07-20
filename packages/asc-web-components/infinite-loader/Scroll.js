import React, { forwardRef } from "react";
import { StyledScroll } from "./StyledInfiniteLoader";

const Scroll = forwardRef((props, ref) => {
  return <StyledScroll {...props} forwardedRef={ref} />;
});

export default Scroll;
