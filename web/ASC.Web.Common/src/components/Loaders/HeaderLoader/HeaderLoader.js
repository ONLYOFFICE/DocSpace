import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";

const StyledHeader = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 24px 168px 1fr 24px 36px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding: 16px;
`;

const StyledSpacer = styled.div``;

const HeaderLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    x,
    y,
    borderRadius,
    backgroundColor,
    foregroundColor,
    backgroundOpacity,
    foregroundOpacity,
    speed,
    animate,
  } = rest;

  return (
    <StyledHeader id={id} className={className} style={style}>
      <RectangleLoader
        title={title}
        x={x}
        y={y}
        width="24"
        height="24"
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
        width="168"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledSpacer />
      <RectangleLoader
        title={title}
        x={x}
        y={y}
        width="24"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledSpacer />
    </StyledHeader>
  );
};

HeaderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

HeaderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  backgroundColor: "#fff",
  foregroundColor: "#fff",
  backgroundOpacity: 0.25,
  foregroundOpacity: 0.2,
};

export default HeaderLoader;
