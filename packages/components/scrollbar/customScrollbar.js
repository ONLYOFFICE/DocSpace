import React from "react";
import StyledScrollbar from "./styled-customScrollbar";
import { useTheme } from "styled-components";

const CustomScrollbars = ({ children, className, ...props }) => {
  const theme = useTheme();

  return (
    <StyledScrollbar {...props}>
      <div
        className="container"
        {...props}
        style={{ overflow: "scroll", direction: theme.interfaceDirection }}
      >
        {children}
      </div>
    </StyledScrollbar>
  );
};

export default CustomScrollbars;
