import React from "react";
import PropTypes from "prop-types";
import StyledHeading from "./styled-heading"


const Heading = ({ level, color, ...rest }) => {
  return (
    <StyledHeading as={`h${level}`} colorProp={color} {...rest}></StyledHeading>
  );
};

Heading.propTypes = {
  level: PropTypes.oneOf([1, 2, 3, 4, 5, 6]),
  color: PropTypes.string,
  title: PropTypes.string,
  truncate: PropTypes.bool,
  isInline: PropTypes.bool,
  size: PropTypes.oneOf(["xsmall", "small", "medium", "large", "xlarge"]),
  className: PropTypes.string,
};

Heading.defaultProps = {
  color: "#333333",
  title: null,
  truncate: false,
  isInline: false,
  size: "large",
  level: 1,
  className: "",
};

export default Heading;
