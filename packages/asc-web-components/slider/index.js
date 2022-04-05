import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { StyledSlider } from "./styled-slider";

const Slider = ({
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
}) => {
  const [size, setSize] = useState("0%");

  useEffect(() => {
    setSize(((value - min) * 100) / (max - min) + "%");
  }, [value]);

  return (
    <StyledSlider
      style={style}
      id={id}
      type={type}
      className={className}
      min={min}
      max={max}
      step={step}
      value={value}
      size={value && withPouring ? size : "0%"}
      withPouring={withPouring}
      onChange={onChange}
    />
  );
};

Slider.propTypes = {
  id: PropTypes.string,
  type: PropTypes.string,
  className: PropTypes.string,
  onChange: PropTypes.func,
  min: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  max: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  step: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  value: PropTypes.number,
  withPouring: PropTypes.bool,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default Slider;
