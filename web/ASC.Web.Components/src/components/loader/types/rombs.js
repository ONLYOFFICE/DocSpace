import React from "react";
import PropTypes from "prop-types";
import styled, { keyframes } from "styled-components";

export const keyFrameBlue = keyframes`
    0%   { top:120px; background: #de7a59;  }
    10%  { top:120px; background: #F2CBBF;  }
    14%  { background: #fff; top: 120px;    }  
    15%  { background: #fff; top: 0;        }  
    20%  { background: #E6E4E4;             }
    30%  { background: #D2D2D2;             }
    40%  { top: 120px;                      }  
    100% { top: 120px; background: #de7a59; }   
`;

export const keyFrameRed = keyframes`
    0%   { top:100px; background: #55bce6;  opacity: 1;   }
    10%  { top:100px; background: #BFE8F8;  opacity: 1;   }
    14%  { background: #fff;  top: 100px;   opacity: 1;   } 
    15%  { background: #fff;  top: 0;       opacity: 1;   }  
    20%  { background: #ffffff; top: 0;     opacity: 0;   }
    25%  { background: #ffffff; top: 0;     opacity: 0;   }
    45%  { background: #EFEFEF; top: 0;     opacity: 0,2; }
    100% { top: 100px; background: #55bce6;               }  
`;

export const keyFrameGreen = keyframes`
    0%   { top:110px; background: #a1cb5c; opacity: 1;  }
    10%  { top:110px; background: #CBE0AC; opacity: 1;  }
    14%  { background: #fff; top: 110px; opacity: 1;    } 
    15%  { background: #fff; top: 0; opacity: 1;        }   
    20%  { background: #ffffff; top: 0; opacity: 0;     }
    25%  { background: #EFEFEF; top: 0; opacity: 1;     }
    30%  { background: #E6E4E4;                         }
    70%  { top: 110px;                                  }
    100% { top: 110px; background: #a1cb5c;             } 
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
      (props.color === "blue" && keyFrameBlue) ||
      (props.color === "red" && keyFrameRed) ||
      (props.color === "green" && keyFrameGreen)}
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
