import styled, { keyframes, css } from 'styled-components';

const color = '#2DA7DB';

const StyledCircleWrap = styled.div`
  width: 48px;
  height: 48px;
  background: ${(props) => props.color};
  border-radius: 50%;
  cursor: pointer;
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
    width: 44px;
    height: 44px;
    position: absolute;
    border-radius: 50%;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    margin: auto;
  }

  ${(props) =>
    props.percent > 0
      ? css`
          .circle__mask {
            clip: rect(0px, 44px, 44px, 22px);
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
    clip: rect(0px, 22px, 44px, 0px);
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
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: ${(props) => (props.color ? props.color : '#fff')};
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  text-align: center;
  margin: 4px;
  position: absolute;
`;

const IconBox = styled.div`
  padding-top: 12px;
`;

const StyledAlertIcon = styled.div`
  position: absolute;
  width: 12px;
  height: 12px;
  left: 26px;
  top: -10px;
`;

export { StyledCircleWrap, StyledCircle, StyledFloatingButton, StyledAlertIcon, IconBox };
