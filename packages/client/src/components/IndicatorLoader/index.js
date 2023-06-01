import React from "react";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const IndicatorLoader = () => {
  return (
    <ColorTheme themeId={ThemeType.IndicatorLoader}>
      <div id="ipl-progress-indicator"></div>
    </ColorTheme>
  );
};

export default IndicatorLoader;
