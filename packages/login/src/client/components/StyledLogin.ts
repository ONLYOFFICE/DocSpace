import styled, { css } from "styled-components";
import { tablet } from "@docspace/components/utils/device";

export const ButtonsWrapper = styled.div`
  display: flex;
  flex-direction: column;
  margin: 0 213px 0 213px;
  width: 320px;

  @media ${tablet} {
    width: 100%;
  }
`;



interface ILoginFormWrapperProps {
  enabledJoin?: boolean;
  isDesktop?: boolean;
  bgPattern?: string;
}

export const LoginFormWrapper = styled.div`
  display: grid;
  grid-template-rows: ${(props: ILoginFormWrapperProps) =>
    props.enabledJoin
      ? props.isDesktop
        ? css`1fr 10px`
        : css`1fr 68px`
      : css`1fr`};
  width: 100%;
  height: 100vh;

  background-image: ${props => props.bgPattern};
  background-repeat: no-repeat;
  background-attachment: fixed;
  background-size: 100% 100%;

  @media (max-width: 1024px) {
    background-size: cover;
  }

  @media (max-width: 428px) {
    background-image: none;
    height: calc(100vh - 48px);
  }

`;
