import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import CircleLoader from "../CircleLoader/index";

import { utils } from "asc-web-components";

const { desktop, tablet, mobile } = utils.device;

const StyledBox1 = styled.div`
  display: grid;
  grid-template-columns: 160px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 32px;

  @media ${mobile} {
    grid-template-columns: 1fr;
    grid-template-rows: 1fr 1fr;
  }
  padding-bottom: 12px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 160px 36px;
  grid-row-gap: 12px;

  padding-bottom: 40px;

  @media ${mobile} {
    padding-bottom: 32px;
  }
`;

const StyledBox3 = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(9, 1fr);
  grid-row-gap: 8px;
  padding-bottom: 40px;
`;

const StyledBox4 = styled.div`
  display: grid;
  grid-template-columns: repeat(2, 200px);
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  padding-top: 40px;
  padding-bottom: 40px;

  @media ${desktop} {
    grid-template-columns: repeat(3, 200px);
  }
  @media ${tablet} {
    .row-content__tablet {
      display: none;
    }
  }
  @media ${mobile} {
    grid-template-columns: 200px;
    .row-content__mobile {
      display: none;
    }
  }
`;

const StyledSpacer = styled.div`
  padding-bottom: 40px;
`;

const ProfileViewLoader = ({ id, className, style, isEdit, ...rest }) => {
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
    <div id={id} className={className} style={style}>
      <StyledBox1>
        <StyledBox2>
          <CircleLoader
            title={title}
            x="80"
            y="80"
            radius="80"
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />
          {isEdit ? (
            <RectangleLoader
              title={title}
              width="160"
              height="36"
              borderRadius={borderRadius}
              backgroundColor={backgroundColor}
              foregroundColor={foregroundColor}
              backgroundOpacity={backgroundOpacity}
              foregroundOpacity={foregroundOpacity}
              speed={speed}
              animate={animate}
            />
          ) : (
            <></>
          )}
        </StyledBox2>
        <StyledBox3>
          <RectangleLoader
            title={title}
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="231"
            height="16"
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
            width="111"
            height="16"
            borderRadius={borderRadius}
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />
        </StyledBox3>
        <RectangleLoader
          title={title}
          width="200"
          height="24"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox1>
      <RectangleLoader
        title={title}
        width="100%"
        height="80"
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
        width="200"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledBox4>
        <RectangleLoader
          title={title}
          width="200"
          height="80"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
        <RectangleLoader
          className="row-content__mobile"
          title={title}
          width="200"
          height="80"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
        <RectangleLoader
          className="row-content__tablet row-content__mobile"
          title={title}
          width="200"
          height="80"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox4>

      <RectangleLoader
        title={title}
        width="200"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
      <StyledBox4>
        <RectangleLoader
          title={title}
          width="200"
          height="80"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
        <RectangleLoader
          className="row-content__mobile"
          title={title}
          width="200"
          height="80"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </StyledBox4>
    </div>
  );
};

ProfileViewLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  isEdit: PropTypes.bool,
};

ProfileViewLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  isEdit: true,
};

export default ProfileViewLoader;
