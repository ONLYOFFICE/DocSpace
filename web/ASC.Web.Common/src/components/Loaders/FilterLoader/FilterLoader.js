import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import { utils } from "asc-web-components";
const { mobile } = utils.device;

const StyledFilter = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr 95px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;

  @media ${mobile} {
    grid-template-columns: 1fr 50px;
  }
`;

const FilterLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    x,
    y,
    height,
    borderRadius,
    backgroundColor,
    foregroundColor,
    backgroundOpacity,
    foregroundOpacity,
    speed,
    animate,
  } = rest;

  return (
    <StyledFilter id={id} className={className} style={style}>
      <RectangleLoader
        title={title}
        x={x}
        y={y}
        height={height}
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <RectangleLoader
        title={title}
        x={x}
        y={y}
        height={height}
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
    </StyledFilter>
  );
};

FilterLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

FilterLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default FilterLoader;
