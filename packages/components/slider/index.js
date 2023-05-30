import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const Slider = (props) => {
  const {
    id,
    type,
    className,
    onChange,
    min,
    max,
    step,
    value,
    withPouring,
    style,
    isReadOnly = false,
    isDisabled = false,
    thumbBorderWidth,
    thumbHeight,
    thumbWidth,
    runnableTrackHeight,
  } = props;
  const [size, setSize] = useState("0%");

  useEffect(() => {
    setSize(((value - min) * 100) / (max - min) + "%");
  }, [value]);

  return (
    <ColorTheme
      {...props}
      themeId={ThemeType.Slider}
      type={type}
      isDisabled={isDisabled}
      style={style}
      id={id}
      className={className}
      min={min}
      max={max}
      step={step}
      value={value}
      size={value && withPouring ? size : "0%"}
      withPouring={withPouring}
      onChange={onChange}
      isReadOnly={isReadOnly}
      thumbBorderWidth={thumbBorderWidth}
      thumbHeight={thumbHeight}
      thumbWidth={thumbWidth}
      runnableTrackHeight={runnableTrackHeight}
    />
  );
};

Slider.propTypes = {
  /** Accepts id */
  id: PropTypes.string,
  /** Sets the input type. Fixed as range.*/
  type: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Sets the width of the input thumb */
  thumbWidth: PropTypes.string,
  /** Sets the height of the input thumb */
  thumbHeight: PropTypes.string,
  /** Sets the border width of the input thumb */
  thumbBorderWidth: PropTypes.string,
  /** Sets the height of the runnableTrack for the input */
  runnableTrackHeight: PropTypes.string,
  /** The change event is triggered when the elelment's value is modified */
  onChange: PropTypes.func,
  /** Default input value */
  min: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Default input value */
  max: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Default input value */
  step: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Default input value */
  value: PropTypes.number,
  /** Sets the background color of the runnableTrack */
  withPouring: PropTypes.bool,
  /** Default input value */
  isReadOnly: PropTypes.bool,
  /** Disables the input  */
  isDisabled: PropTypes.bool,
  /** Accepts css */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default Slider;
