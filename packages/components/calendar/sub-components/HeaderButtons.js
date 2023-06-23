import React from "react";
import { ButtonsContainer, ArrowIcon } from "../styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import { useTheme } from "styled-components";

export const HeaderButtons = ({
  onLeftClick,
  onRightClick,
  isLeftDisabled,
  isRightDisabled,
}) => {
  const theme = useTheme();
  const isRtl = theme.interfaceDirection === "rtl";
  return (
    <ButtonsContainer>
      <ColorTheme
        themeId={ThemeType.RoundButton}
        style={isRtl ? { marginLeft: "12px" } : { marginRight: "12px" }}
        onClick={onLeftClick}
        disabled={isLeftDisabled}
      >
        <ArrowIcon previous />
      </ColorTheme>

      <ColorTheme
        themeId={ThemeType.RoundButton}
        onClick={onRightClick}
        disabled={isRightDisabled}
      >
        <ArrowIcon next />
      </ColorTheme>
    </ButtonsContainer>
  );
};
