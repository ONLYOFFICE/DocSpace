import React from "react";
import { LoaderStyle } from "../../../constants";
import RectangleLoader from "../RectangleLoader";

const speed = 2;

const SettingsHeaderLoader = () => (
  <RectangleLoader
    height={24}
    width={140}
    backgroundColor={LoaderStyle.backgroundColor}
    foregroundColor={LoaderStyle.foregroundColor}
    backgroundOpacity={LoaderStyle.backgroundOpacity}
    foregroundOpacity={LoaderStyle.foregroundOpacity}
    speed={speed}
    animate={true}
  />
);

export default SettingsHeaderLoader;
