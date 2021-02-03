import React from "react";
import MaskedInput from "react-text-mask";



/* eslint-disable no-unused-vars, react/prop-types */
const Input = ({
    isAutoFocussed,
    isDisabled,
    isReadOnly,
    hasError,
    hasWarning,
    scale,
    withBorder,
    keepCharPositions,
    fontWeight,
    isBold,
    ...props
  }) =>
    props.mask != null ? (
      <MaskedInput keepCharPositions {...props} />
    ) : (
      <input {...props} />
    );

    export default Input;