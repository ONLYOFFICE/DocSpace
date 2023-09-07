import React from "react";
import { ButtonsContainer, ArrowIcon } from "../styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import { useTheme } from "styled-components";

export const HeaderButtons = ({
  onLeftClick,
  onRightClick,
  isLeftDisabled,
  isRightDisabled,
  isMobile,
}) => {
  const theme = useTheme();
  const isRtl = theme.interfaceDirection === "rtl";
  const marginSize = isMobile ? "12px" : "8px";
  return (
    <ButtonsContainer>
      <ColorTheme
        className="arrow-previous"
        themeId={ThemeType.RoundButton}
        style={isRtl ? { marginLeft: marginSize } : { marginRight: marginSize }}
        onClick={onLeftClick}
        disabled={isLeftDisabled}
        isMobile={isMobile}
      >
        <ArrowIcon previous isMobile={isMobile} />
      </ColorTheme>

      <ColorTheme
        className="arrow-next"
        themeId={ThemeType.RoundButton}
        onClick={onRightClick}
        disabled={isRightDisabled}
        isMobile={isMobile}
      >
        <ArrowIcon next isMobile={isMobile} />
      </ColorTheme>
    </ButtonsContainer>
  );
};
