import {
  ButtonsContainer,
  RoundButton,
  PrevIcon,
  NextIcon,
} from "../styled-components";

export const HeaderButtons = ({ onLeftClick, onRightClick }) => {
  return (
    <ButtonsContainer>
      <RoundButton style={{ marginRight: "12px" }} onClick={onLeftClick}>
        <PrevIcon />
      </RoundButton>

      <RoundButton onClick={onRightClick}>
        <NextIcon />
      </RoundButton>
    </ButtonsContainer>
  );
};
