import React from "react";
import PropTypes from "prop-types";
import Text from "../text";

const Label = (props) => {
  const {
    isRequired,
    error,
    title,
    truncate,
    isInline,
    htmlFor,
    text,
    display,
    className,
    id,
    style,
    theme,
  } = props;
  const errorProp = error ? { color: "#c30" } : {};

  return (
    <Text
      as="label"
      id={id}
      style={style}
      htmlFor={htmlFor}
      isInline={isInline}
      display={display}
      {...errorProp}
      fontWeight={600}
      truncate={truncate}
      title={title}
      className={className}
    >
      {text} {isRequired && " *"}
    </Text>
  );
};

Label.propTypes = {
  /** Indicates that the field to which the label is attached is required to fill */
  isRequired: PropTypes.bool,
  /** Indicates that the field to which the label is attached is incorrect */
  error: PropTypes.bool,
  /** Sets the 'display: inline-block' property */
  isInline: PropTypes.bool,
  /** Title */
  title: PropTypes.string,
  /** Disables word wrapping */
  truncate: PropTypes.bool,
  /** The field ID to which the label is attached */
  htmlFor: PropTypes.string,
  /** Text */
  text: PropTypes.string,
  /** Sets the 'display' property */
  display: PropTypes.string,
  /** Class name */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Label.defaultProps = {
  isRequired: false,
  error: false,
  isInline: false,
  truncate: false,
};

export default Label;
