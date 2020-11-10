import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import CircleLoader from "../CircleLoader/index";

import { utils } from "asc-web-components";
const { desktop } = utils.device;

const StyledRow = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 16px 22px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: ${(props) => props.gap || "8px"};
  margin-bottom: 32px;
  justify-items: center;
  align-items: center;
`;

const StyledBox1 = styled.div`
  .rectangle-content {
    width: 32px;
    height: 32px;
  }

  @media ${desktop} {
    .rectangle-content {
      width: 22px;
      height: 22px;
    }
  }
`;

const StyledBox2 = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 16px;
  grid-row-gap: 4px;
  justify-items: left;
  align-items: left;

  .first-row-content__mobile {
    width: 80%;
  }

  @media ${desktop} {
    grid-template-rows: 16px;
    grid-row-gap: 0;

    .first-row-content__mobile {
      width: 100%;
    }

    .second-row-content__mobile {
      width: 100%;
      display: none;
    }
  }
`;

const Row = ({ id, className, style, isRectangle, ...rest }) => {
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
    <StyledRow
      id={id}
      className={className}
      style={style}
      gap={isRectangle ? "8px" : "16px"}
    >
      <RectangleLoader
        title={title}
        x={x}
        y={y}
        width="16"
        height="16"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledBox1>
        {isRectangle ? (
          <RectangleLoader
            className="rectangle-content"
            title={title}
            x={x}
            y={y}
            width="100%"
            height="100%"
            borderRadius={borderRadius}
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />
        ) : (
          <CircleLoader
            title={title}
            x="16"
            y="16"
            width="32"
            height="32"
            radius="16"
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />
        )}
      </StyledBox1>
      <StyledBox2>
        <RectangleLoader
          className="first-row-content__mobile"
          title={title}
          x={x}
          y={y}
          height="16px"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
        <RectangleLoader
          className="second-row-content__mobile"
          title={title}
          x={x}
          y={y}
          height="12px"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox2>
    </StyledRow>
  );
};

const RowsLoader = (props) => {
  return (
    <div>
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
    </div>
  );
};

Row.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  isRectangle: PropTypes.bool,
};

Row.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  isRectangle: true,
};

export default RowsLoader;
