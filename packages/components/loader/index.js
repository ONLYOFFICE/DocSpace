import React from "react";
import PropTypes from "prop-types";

import { Oval } from "./types/oval";
import { DualRing } from "./types/dual-ring";
import { Rombs } from "./types/rombs";
import { Track } from "./types/track";

import Text from "../text";

const Loader = (props) => {
  const {
    type,
    color,
    size,
    label,
    className,
    style,
    id,
    primary,
    theme,
  } = props;

  const svgRenderer = (type) => {
    switch (type) {
      case "oval":
        return <Oval {...props} />;
      case "dual-ring":
        return <DualRing {...props} />;
      case "rombs":
        return <Rombs {...props} />;
      case "track":
        return <Track {...props} />;
      default:
        return (
          <span style={{ ...style }}>
            <Text color={color} fontSize={size}>
              {label}
            </Text>
          </span>
        );
    }
  };

  return (
    <div aria-busy="true" className={className} style={style} id={id}>
      {svgRenderer(type)}
    </div>
  );
};

Loader.propTypes = {
  /** Font color */
  color: PropTypes.string,
  /** Type loader */
  type: PropTypes.oneOf(["base", "oval", "dual-ring", "rombs", "track"]),
  /** Font size  */
  size: PropTypes.string,
  /** Text label */
  label: PropTypes.string,
  /** Class name */
  className: PropTypes.string,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Loader.defaultProps = {
  type: "base",
  size: "40px",
  label: "Loading content, please wait.",
};

export default Loader;
