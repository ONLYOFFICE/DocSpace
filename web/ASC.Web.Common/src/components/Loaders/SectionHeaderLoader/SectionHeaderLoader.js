import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";

import { utils } from "asc-web-components";
const { mobile, tablet } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 100px 0fr 42px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  margin-top: 20px;
  margin-bottom: 18px;

  @media ${mobile}, ${tablet} {
    grid-template-columns: 100px 1fr 42px;
  }
`;

const StyledBox1 = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 17px 67px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 17px 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
`;

const StyledSpacer = styled.div``;

const SectionHeaderLoader = ({ id, className, style, ...rest }) => {
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
    <StyledContainer id={id} className={className} style={style}>
      <StyledBox1>
        <RectangleLoader
          title={title}
          x={x}
          y={y}
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
          x={x}
          y={y}
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
          x={x}
          y={y}
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
          x={x}
          y={y}
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
