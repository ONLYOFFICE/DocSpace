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
  forwardedRef,
  className,
  theme,
  ...props
}) => {
  const rest = {};

  if (isAutoFocussed) rest.autoFocus = true;

  if (forwardedRef) rest.ref = forwardedRef;

  return props.mask != null ? (
    <MaskedInput
      className={`${className} not-selectable`}
      keepCharPositions
      {...props}
    />
  ) : (
    <input className={`${className} not-selectable`} {...props} {...rest} />
  );
};
export default Input;
