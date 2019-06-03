
import React from "react";
import PropTypes from "prop-types";

import { Oval } from "./items/oval";
import { DualRing } from "./items/dual-ring";

const Loader = (props) =>  {
    const { type, color, label } = props;
  
    const svgRenderer = type => {
      switch (type) {
        case "oval":
          return <Oval {...props} />;
        case "dual-ring":
          return <DualRing {...props} />;
        default:
          return (
            <div aria-busy="true">
              <span style={{ color: color }}>{label}</span>
            </div>
          );
      }
    };
  
    return (
        <div aria-busy="true">{svgRenderer(type)}</div>
    );
  };

  Loader.propTypes = {
    color: PropTypes.string,
    type: PropTypes.oneOf(['base', 'oval', 'dual-ring']),
    height: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    width: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    label: PropTypes.string
  };

  Loader.defaultProps = {
    color: "#71238",
    type: "base",
    height: 80,
    width: 80,
    label: "Loading content, please wait."
  };

  export default Loader;