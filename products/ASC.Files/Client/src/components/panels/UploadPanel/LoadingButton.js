import React from "react";
import styled from "styled-components";

const backgroundColor = "none";
const color = "#2DA7DB";

const StyledCircleWrap = styled.div`
  width: 16px;
  height: 16px;
  background: ${backgroundColor};
  border-radius: 50%;
  cursor: pointer;
`;

const StyledCircle = styled.div`
  .circle__mask,
  .circle__fill {
    width: 16px;
    height: 16px;
    position: absolute;
    border-radius: 50%;
  }

  .circle__mask {
    clip: rect(0px, 16px, 16px, 8px);
  }

  .circle__fill {
    animation: fill-rotate ease-in-out none;
    transform: rotate(${(props) => props.percent * 1.8}deg);
  }

  .circle__mask .circle__fill {
    clip: rect(0px, 8px, 16px, 0px);
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

const StyledLoadingButton = styled.div`
  width: 12px;
  height: 12px;
  border-radius: 50%;
  text-align: center;
  line-height: 12px;
  background: #fff;
  position: absolute;
  margin: 2px;
  color: #2da7db;
  font-size: 16px;
  font-weight: bold;
`;

const LoadingButton = ({ id, className, style, ...rest }) => {
  const { percent, onClick } = rest;

  return (
    <StyledCircleWrap
      id={id}
      className={className}
      style={style}
      onClick={onClick}
    >
      <StyledCircle percent={percent}>
        <div className="circle__mask circle__full">
          <div className="circle__fill"></div>
        </div>
        <div className="circle__mask">
          <div className="circle__fill"></div>
        </div>

        <StyledLoadingButton>&times;</StyledLoadingButton>
      </StyledCircle>
    </StyledCircleWrap>
  );
};

export default LoadingButton;
