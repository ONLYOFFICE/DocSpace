import {
  HeaderContainer,
  Title,
  ButtonsContainer,
  RoundButton,
  PrevIcon,
  NextIcon,
} from "../styled-components";

export const Header = () => {
  return (
    <HeaderContainer>
      <Title>January 2023</Title>

      <ButtonsContainer>
        <RoundButton style={{ marginRight: "12px" }}>
          <PrevIcon />
        </RoundButton>

        <RoundButton>
          <NextIcon />
        </RoundButton>
      </ButtonsContainer>
    </HeaderContainer>
  );
};
