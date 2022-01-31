import styled, { keyframes, css } from "styled-components";

const color = "#2DA7DB";

const StyledCircleWrap = styled.div`
  width: 48px;
  height: 48px;
  background: ${(props) => (props.color ? props.color : "#fff")};
  border-radius: 50%;
  cursor: pointer;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const rotate360 = keyframes`
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
`;

const StyledCircle = styled.div`
  .circle__mask,
  .circle__fill {
    width: 42px;
    height: 42px;
    position: absolute;
    border-radius: 50%;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    margin: auto;

    display: flex;
    align-items: center;
    justify-content: center;
  }

  ${(props) =>
    props.percent > 0
      ? css`
          .circle__mask {
            clip: rect(0px, 42px, 42px, 21px);
          }

          .circle__fill {
            animation: fill-rotate ease-in-out none;
            transform: rotate(${(props) => props.percent * 1.8}deg);
          }
        `
      : css`
          .circle__fill {
            animation: ${rotate360} 2s linear infinite;
            transform: translate(0);
          }
        `}

  .circle__mask .circle__fill {
    clip: rect(0px, 21px, 42px, 0px);
    background-color: ${color};
  }

  .circle__mask.circle__full {
    animation: fill-rotate ease-in-out none;
    transform: rotate(${(props) => props.percent * 1.8}deg);
  }

  @keyframes fill-rotate {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(${(props) => props.percent * 1.8}deg);
    }
  }
`;

const StyledFloatingButton = styled.div`
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: ${(props) => (props.color ? props.color : "#fff")};
  text-align: center;
  margin: 5px;
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
`;

const IconBox = styled.div`
  // padding-top: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
`;

const StyledAlertIcon = styled.div`
  position: absolute;
  width: 12px;
  height: 12px;
  left: 19px;
  top: 0px;
`;

export {
  StyledCircleWrap,
  StyledCircle,
  StyledFloatingButton,
  StyledAlertIcon,
  IconBox,
};
