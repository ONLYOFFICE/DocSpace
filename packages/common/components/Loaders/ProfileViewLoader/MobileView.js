import React from "react";
import { MobileView } from "./StyledProfileView";
import RectangleLoader from "../RectangleLoader";
import CircleLoader from "../CircleLoader";

const MobileViewLoader = ({ ...rest }) => {
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
    <MobileView>
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
      <div className="info">
        <RectangleLoader
          title={title}
          height="58"
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
          height="58"
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
          height="58"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </div>
      <div className="block">
        <RectangleLoader
          title={title}
          width="78"
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
          height="32"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </div>

      <div className="notifications">
        <RectangleLoader
          title={title}
          width="101"
          height="22"
          className="title"
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
          height="32"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </div>

      <div className="theme">
        <RectangleLoader
          title={title}
          className="theme-title"
          width="129"
          height="22"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />

        <div className="flex">
          <RectangleLoader
            title={title}
            width="16"
            height="16"
            className="check-box"
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
            width="124"
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

        <div className="theme-selection">
          <RectangleLoader
            title={title}
            width="291"
            height="32"
            className="theme-description"
            borderRadius={borderRadius}
            backgroundColor={backgroundColor}
            foregroundColor={foregroundColor}
            backgroundOpacity={backgroundOpacity}
            foregroundOpacity={foregroundOpacity}
            speed={speed}
            animate={animate}
          />

          <div className="check-box-container">
            <div className="flex">
              <CircleLoader
                title={title}
                className="check-box"
                x="8"
                y="8"
                radius="8"
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
              <RectangleLoader
                title={title}
                width="124"
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
            <div className="flex">
              <CircleLoader
                title={title}
                className="check-box"
                x="8"
                y="8"
                radius="8"
                backgroundColor={backgroundColor}
                foregroundColor={foregroundColor}
                backgroundOpacity={backgroundOpacity}
                foregroundOpacity={foregroundOpacity}
                speed={speed}
                animate={animate}
              />
              <RectangleLoader
                title={title}
                width="124"
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
        </div>
      </div>
    </MobileView>
  );
};

export default MobileViewLoader;
