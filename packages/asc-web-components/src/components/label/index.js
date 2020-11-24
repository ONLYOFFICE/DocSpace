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
  isRequired: PropTypes.bool,
  error: PropTypes.bool,
  isInline: PropTypes.bool,
  title: PropTypes.string,
  truncate: PropTypes.bool,
  htmlFor: PropTypes.string,
  text: PropTypes.string,
  display: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Label.defaultProps = {
  isRequired: false,
  error: false,
  isInline: false,
  truncate: false,
};

export default Label;
