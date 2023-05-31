import React from "react";
import { ButtonsContainer, ArrowIcon } from "../styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

export const HeaderButtons = ({
  onLeftClick,
  onRightClick,
  isLeftDisabled,
  isRightDisabled,
}) => {
  return (
    <ButtonsContainer>
      <ColorTheme
        themeId={ThemeType.RoundButton}
        style={{ marginRight: "12px" }}
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
