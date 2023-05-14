import React from "react";
import { ButtonsContainer, PrevIcon, NextIcon } from "../styled-components";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

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
        <PrevIcon />
      </ColorTheme>

      <ColorTheme
        themeId={ThemeType.RoundButton}
        onClick={onRightClick}
        disabled={isRightDisabled}
      >
        <NextIcon />
      </ColorTheme>
    </ButtonsContainer>
  );
};
