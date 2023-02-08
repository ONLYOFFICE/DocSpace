import React from "react";
import PropTypes from "prop-types";
import StyledHeading from "./StyledHeadline";
import { classNames } from "@docspace/components/utils/classNames";

const Headline = ({ type, className, ...props }) => {
  //console.log("Headline render");
  return (
    <StyledHeading
      headlineType={type}
      className={classNames("headline-heading", className)}
      {...props}
    />
  );
};

Headline.propTypes = {
  level: PropTypes.oneOf([1, 2, 3, 4, 5, 6]),
  children: PropTypes.any,
  color: PropTypes.string,
  title: PropTypes.string,
  truncate: PropTypes.bool,
  isInline: PropTypes.bool,
  type: PropTypes.oneOf(["content", "header", "menu"]),
};

Headline.defaultProps = {
  title: null,
  truncate: false,
  isInline: false,
  level: 1,
};

export default Headline;
