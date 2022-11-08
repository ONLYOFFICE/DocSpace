import React from "react";
import PropTypes from "prop-types";
import { StyledWrapper, MainBlock } from "./StyledProfileView";
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
      <StyledWrapper>
        <MainBlock>
          <CircleLoader
            className="avatar"
            title={title}
            x="62"
            y="62"
            radius="62"
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />

          <div className="combos">
            <div className="row">
              <RectangleLoader
                title={title}
                width="37"
                height="20"
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
                width="226"
                height="20"
                borderRadius={borderRadius}
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
            </div>
            <div className="row">
              <RectangleLoader
                title={title}
                width="34"
                height="20"
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
                width="156"
                height="20"
                borderRadius={borderRadius}
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
            </div>
            <div className="row">
              <RectangleLoader
                title={title}
                width="59"
                height="20"
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
                width="93"
                height="20"
                borderRadius={borderRadius}
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
            </div>
            <div className="row">
              <RectangleLoader
                title={title}
                width="75"
                height="20"
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
                width="208"
                height="20"
                borderRadius={borderRadius}
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
            </div>
            <div className="row">
              <RectangleLoader
                title={title}
                width="59"
                height="20"
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
                width="140"
                height="20"
                borderRadius={borderRadius}
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
            </div>
          </div>
        </MainBlock>
      </StyledWrapper>
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
