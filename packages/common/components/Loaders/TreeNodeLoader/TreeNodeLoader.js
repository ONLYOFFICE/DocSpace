import React from "react";
import RectangleLoader from "../RectangleLoader";
import CircleLoader from "../CircleLoader";

const TreeNodeLoader = ({
  title,
  borderRadius,
  backgroundColor,
  foregroundColor,
  backgroundOpacity,
  foregroundOpacity,
  speed,
  animate,
  withRectangle = false,
}) => {
  return (
    <>
      <CircleLoader
        title={title}
        height="32"
        radius="3"
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />

      {withRectangle && (
        <RectangleLoader
          title={title}
          width="16"
          height="16"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
          className="tree-node-loader_additional-rectangle"
        />
      )}

      <RectangleLoader
        title={title}
        width="100%"
        height="24"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate={animate}
      />
    </>
  );
};

export default TreeNodeLoader;
