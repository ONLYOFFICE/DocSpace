import React from "react";
import PropTypes from "prop-types";
import {
  StyledBox1,
  StyledBox2,
  StyledBox3,
  StyledBox4,
  StyledSpacer,
} from "./StyledProfileView";
import RectangleLoader from "../RectangleLoader";
import CircleLoader from "../CircleLoader";

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
        className="rectangle-loader"
        title={title}
        width="100%"
        height="80"
        style={{ maxWidth: "420px" }}
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
