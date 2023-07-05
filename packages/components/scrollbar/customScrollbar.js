import React from "react";
import StyledScrollbar from "./styled-customScrollbar";
import { useTheme } from "styled-components";

const CustomScrollbars = React.forwardRef((props, ref) => {
  const theme = useTheme();
  return (
    <StyledScrollbar className={props.className} {...props}>
      <div
        className={`container  scroll-body ${props.scrollclass}`}
        {...props}
        style={{ overflow: "scroll", direction: theme.interfaceDirection }}
        ref={ref}
      >
        {props.children}
      </div>
    </StyledScrollbar>
  );
});

export default CustomScrollbars;
