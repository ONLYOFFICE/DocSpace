import React from "react";
import PropTypes from "prop-types";
import styled, { keyframes } from "styled-components";

export const keyFrameBlue = (props) => keyframes`
    0%   { background: #de7a59;               top: 120px; }
    10%  { background: ${props.colorStep_1};  top: 120px; }
    14%  { background: ${props.colorStep_2};  top: 120px; }
    15%  { background: ${props.colorStep_2};  top: 0;     }
    20%  { background: ${props.colorStep_3};              }
    30%  { background: ${props.colorStep_4};              }
    40%  { top: 120px;                                    }
    100% { background: #de7a59;               top: 120px; }
`;

export const keyFrameRed = (props) => keyframes`
    0%   { background: #55bce6;               top: 100px; opacity: 1; }
    10%  { background: ${props.colorStep_1};  top: 100px; opacity: 1; }
    14%  { background: ${props.colorStep_2};  top: 100px; opacity: 1; }
    15%  { background: ${props.colorStep_2};  top: 0;     opacity: 1; }
    20%  { background: ${props.colorStep_2};  top: 0;     opacity: 0; }
    45%  { background: ${props.colorStep_3};  top: 0;                 }
    100% { background: #55bce6;               top: 100px;             }
`;

export const keyFrameGreen = (props) => keyframes`
    0%   { background: #a1cb5c;               top: 110px; opacity: 1; }
    10%  { background: ${props.colorStep_1};  top: 110px; opacity: 1; }
    14%  { background: ${props.colorStep_2};  top: 110px; opacity: 1; }
    15%  { background: ${props.colorStep_2};  top: 0;     opacity: 1; }
    20%  { background: ${props.colorStep_2};  top: 0;     opacity: 0; }
    25%  { background: ${props.colorStep_3};  top: 0;     opacity: 1; }
    30%  { background: ${props.colorStep_4};                          }
    70%  { top: 110px;                                                }
    100% { background: #a1cb5c;               top: 110px;             }
`;

const Romb = styled.div`
  width: ${(props) => props.size};
  height: ${(props) => props.size};
  -ms-transform: rotate(135deg) skew(20deg, 20deg);
  -webkit-transform: rotate(135deg) skew(20deg, 20deg);
  -moz-transform: rotate(135deg) skew(20deg, 20deg);
  -o-transform: rotate(135deg) skew(20deg, 20deg);
  background: red;
  border-radius: 6px;
  animation: movedown 3s infinite ease;
  -moz-animation: movedown 3s infinite ease;
  -webkit-animation: movedown 3s infinite ease;
  -o-animation: movedown 3s infinite ease;
  -ms-animation: movedown 3s infinite ease;
  position: absolute;

  background: ${(props) =>
    (props.color === "blue" && "#55bce6") ||
    (props.color === "red" && "#de7a59") ||
    (props.color === "green" && "#a1cb5c")};

  z-index: ${(props) =>
    (props.color === "blue" && "1") ||
    (props.color === "red" && "3") ||
    (props.color === "green" && "2")};

  animation: ${(props) =>
      (props.color === "blue" && keyFrameBlue(props.theme.rombsLoader.blue)) ||
      (props.color === "red" && keyFrameRed(props.theme.rombsLoader.red)) ||
      (props.color === "green" && keyFrameGreen(props.theme.rombsLoader.green))}
    2s ease-in-out 0s infinite;
`;

Romb.propTypes = {
  size: PropTypes.string.isRequired,
};

// eslint-disable-next-line react/prop-types
const Rombs = ({ size }) => (
  <>
    <Romb color="blue" size={size} />
    <Romb color="green" size={size} />
    <Romb color="red" size={size} />
  </>
);

Rombs.propTypes = {
  size: PropTypes.string.isRequired,
};

Rombs.defaultProps = {
  size: "40px",
};

export { Rombs };
