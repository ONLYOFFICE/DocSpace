import React from "react";
import PropTypes from "prop-types";
import StyledHeading from "./styled-heading";

const Heading = ({ level, color, className, ...rest }) => {
  return (
    <StyledHeading
      className={`${className} not-selectable`}
      as={`h${level}`}
      colorProp={color}
      {...rest}
    ></StyledHeading>
  );
};

Heading.propTypes = {
  /** 	The heading level. It corresponds to the number after the 'H' for the DOM tag. Sets the level for semantic accuracy and accessibility. */
  level: PropTypes.oneOf([1, 2, 3, 4, 5, 6]),
  /** Specifies the headline color */
  color: PropTypes.string,
  /** Title */
  title: PropTypes.string,
  /** Disables word wrapping */
  truncate: PropTypes.bool,
  /** Sets the 'display: inline-block' property */
  isInline: PropTypes.bool,
  /** Sets the size of headline */
  size: PropTypes.oneOf(["xsmall", "small", "medium", "large", "xlarge"]),
  /** Accepts css class */
  className: PropTypes.string,
};

Heading.defaultProps = {
  title: null,
  truncate: false,
  isInline: false,
  size: "large",
  level: 1,
  className: "",
};

export default React.memo(Heading);
