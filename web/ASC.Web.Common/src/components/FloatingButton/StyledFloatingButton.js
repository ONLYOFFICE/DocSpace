import styled from "styled-components";

const backgroundColor = "none";
const color = "#2DA7DB";

const StyledFloatingButton = styled.div`
  .circle-wrap {
    width: 54px;
    height: 54px;
    background: ${backgroundColor};
    border-radius: 50%;
  }
  .circle-wrap .circle .mask,
  .circle-wrap .circle .fill {
    width: 54px;
    height: 54px;
    position: absolute;
    border-radius: 50%;
  }
  .circle-wrap .circle .mask {
    clip: rect(0px, 54px, 54px, 27px);
  }
  .circle-wrap .circle .mask .fill {
    clip: rect(0px, 27px, 54px, 0px);
    background-color: ${color};
  }
  .circle-wrap .circle .mask.full,
  .circle-wrap .circle .fill {
    animation: fill ease-in-out 3s;
    transform: rotate(${(props) => props.percent * 1.8}deg);
  }
  @keyframes fill {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(${(props) => props.percent * 1.8}deg);
    }
  }
  .circle-wrap .inside-circle {
    width: 48px;
    height: 48px;
    border-radius: 50%;
    background: #fff;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    line-height: 48px;
    text-align: center;
    margin: 3px;
    position: absolute;
  }
`;

const StyledAlertIcon = styled.div`
  position: absolute;
  width: 12px;
  height: 12px;
  left: 26px;
  top: -10px;
`;

export { StyledFloatingButton, StyledAlertIcon };
