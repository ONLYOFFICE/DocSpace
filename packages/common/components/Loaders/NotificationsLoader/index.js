import React from "react";
import RectangleLoader from "../RectangleLoader";
import { StyledComponent } from "./StyledComponent";

const NotificationsLoader = ({
  title,
  borderRadius,
  backgroundColor,
  foregroundColor,
  backgroundOpacity,
  foregroundOpacity,
  speed,
  count = 1,
}) => {
  const items = [];

  const contentItem = (
    <>
      <div>
        <RectangleLoader
          className="notifications_title-loader"
          title={title}
          width="100%"
          height="16px"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate
        />
        <RectangleLoader
          className="notifications_content-loader"
          title={title}
          width="100%"
          height="16px"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate
        />
      </div>
      <RectangleLoader
        className="notifications_content-loader"
        title={title}
        width="24px"
        height="16px"
        borderRadius={borderRadius}
        backgroundColor={backgroundColor}
        foregroundColor={foregroundColor}
        backgroundOpacity={backgroundOpacity}
        foregroundOpacity={foregroundOpacity}
        speed={speed}
        animate
      />
    </>
  );

  for (var i = 0; i < count; i++) {
    items.push(<div key={`notification_loader_${i}`}>{contentItem}</div>);
  }
  return <StyledComponent>{items}</StyledComponent>;
};

export default NotificationsLoader;
