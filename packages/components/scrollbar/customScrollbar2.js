import React from "react";
import "./style.css";
const CustomScrollbars = ({ children, className, ...props }) => {
  return (
    <div className="container">
      <div {...props}>{children}</div>
    </div>
  );
};

export default CustomScrollbars;
