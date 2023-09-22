import React from "react";
import { LoaderStyle } from "../../../constants";
import RectangleLoader from "../RectangleLoader";
import { StyledSMTPContent } from "./StyledComponent";
const speed = 2;

const SettingsDSConnect = () => {
  const firstComponent = (
    <div>
      <div>
        <RectangleLoader
          height="22"
          width="56"
          backgroundColor={LoaderStyle.backgroundColor}
          foregroundColor={LoaderStyle.foregroundColor}
          backgroundOpacity={LoaderStyle.backgroundOpacity}
          foregroundOpacity={LoaderStyle.foregroundOpacity}
          speed={speed}
          animate={true}
        />
      </div>

      <RectangleLoader
        className="rectangle-loader-2"
        height="46"
        width="348"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );

  const secondComponent = (
    <div>
      <RectangleLoader
        height="20"
        width="101"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
      <RectangleLoader
        className="rectangle-loader-2"
        height="32"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );
  const thirdComponent = (
    <div>
      <RectangleLoader
        height="20"
        width="138"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
      <RectangleLoader
        className="rectangle-loader-2"
        height="32"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );

  const checkboxComponent = (
    <div className="rectangle-loader_checkbox">
      <RectangleLoader
        height="16"
        width="16"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
      <RectangleLoader
        height="22"
        width="101"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );
  const secondCheckboxComponent = (
    <div className="rectangle-loader_checkbox">
      <RectangleLoader
        height="16"
        width="16"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
      <RectangleLoader
        height="20"
        width="70"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );
  const buttonsComponent = (
    <div className="rectangle-loader_buttons">
      <RectangleLoader
        height="32"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
      <RectangleLoader
        height="32"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />
    </div>
  );
  return (
    <StyledSMTPContent>
      <RectangleLoader
        className="rectangle-loader_title"
        height="22"
        width="128"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />

      <RectangleLoader
        className="rectangle-loader_description"
        height="40"
        backgroundColor={LoaderStyle.backgroundColor}
        foregroundColor={LoaderStyle.foregroundColor}
        backgroundOpacity={LoaderStyle.backgroundOpacity}
        foregroundOpacity={LoaderStyle.foregroundOpacity}
        speed={speed}
        animate={true}
      />

      {firstComponent}
      {firstComponent}
      {firstComponent}

      {buttonsComponent}
    </StyledSMTPContent>
  );
};

export default SettingsDSConnect;
