import React from "react";
import PropTypes from "prop-types";
import {
  StyledContainer,
  StyledBox1,
  StyledBox2,
  StyledSpacer,
} from "./StyledSectionHeaderLoader";
import RectangleLoader from "../RectangleLoader";

const SectionHeaderLoader = ({ id, className, style, ...rest }) => {
  const {
    title,
    borderRadius,
    backgroundColor,
    foregroundColor,
    backgroundOpacity,
    foregroundOpacity,
    speed,
    animate,
  } = rest;

  return (
    <StyledContainer id={id} className={className} style={style}>
      <StyledBox1>
        <RectangleLoader
          title={title}
          width="17"
          height="17"
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
          width="67"
          height="17"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox1>
      <StyledSpacer />
      <StyledBox2>
        <RectangleLoader
          title={title}
          width="17"
          height="17"
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
          width="17"
          height="17"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox2>
    </StyledContainer>
  );
};

SectionHeaderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

SectionHeaderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default SectionHeaderLoader;
