import React from "react";
import {
  ButtonsContainer,
  RoundButton,
  PrevIcon,
  NextIcon,
} from "../styled-components";

export const HeaderButtons = ({
  onLeftClick,
  onRightClick,
  isLeftDisabled,
  isRightDisabled,
}) => {
  return (
    <ButtonsContainer>
      <RoundButton
        style={{ marginRight: "12px" }}
        onClick={onLeftClick}
        disabled={isLeftDisabled}
      >
        <PrevIcon />
      </RoundButton>

      <RoundButton onClick={onRightClick} disabled={isRightDisabled}>
        <NextIcon />
      </RoundButton>
    </ButtonsContainer>
  );
};
