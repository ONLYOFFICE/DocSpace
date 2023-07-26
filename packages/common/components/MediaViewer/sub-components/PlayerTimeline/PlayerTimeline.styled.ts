import { isMobile } from "react-device-detect";
import styled, { css } from "styled-components";

export const HoverProgress = styled.div`
  display: none;
  position: absolute;
  left: 2px;

  height: 6px;

  border-radius: 5px;
  background-color: rgba(255, 255, 255, 0.3);
`;

const mobileCss = css`
  margin-top: 16px;

  input[type="range"]::-webkit-slider-thumb {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: visible;
    opacity: 1;
    background: #fff;
    height: 10px;
    width: 10px;
    border-radius: 50%;
    cursor: pointer;
  }

  input[type="range"]::-moz-range-thumb {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: visible;
    opacity: 1;
    background: #fff;
    height: 10px;
    width: 10px;
    border-radius: 50%;
    cursor: pointer;
    border: none;
  }

  input[type="range"]::-ms-fill-upper {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: visible;
    opacity: 1;
    background: #fff;
    height: 10px;
    width: 10px;
    border-radius: 50%;
    cursor: pointer;
  }
`;

export const PlayerTimelineWrapper = styled.div`
  position: relative;

  display: flex;
  align-items: center;

  margin-top: 12px;

  height: 4px;
  width: 100%;

  cursor: pointer;

  time {
    display: none;
    position: absolute;
    left: 50%;
    top: -25px;
    font-size: 13px;
    color: #fff;
    pointer-events: none;
    transform: translateX(-50%);
  }

  @media (hover: hover) {
    &:hover {
      /* height: 6px; */
      input {
        height: 6px;
      }
      ${HoverProgress} {
        display: block;
      }
      transition: 0.1s height ease-in;
    }

    &:hover time {
      display: block;
    }
  }

  input {
    width: 100%;
    height: 4px;

    outline: none;

    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;

    border-radius: 5px;

    background: rgba(255, 255, 255, 0.3);
    background-image: linear-gradient(#fff, #fff);
    background-repeat: no-repeat;

    z-index: 1;

    &:hover {
      cursor: pointer;
    }
  }

  input[type="range"]::-webkit-slider-thumb {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: hidden;
    opacity: 0;
    background: #fff;
  }

  input[type="range"]::-moz-range-thumb {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: hidden;
    opacity: 0;
    background: #fff;
  }

  input[type="range"]::-ms-fill-upper {
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
    visibility: hidden;
    opacity: 0;
    background: #fff;
  }

  &:hover {
    input[type="range"]::-webkit-slider-thumb {
      visibility: visible;
      height: 12px;
      width: 12px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
    }

    input[type="range"]::-moz-range-thumb {
      visibility: visible;
      height: 12px;
      width: 12px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
      border: none;
    }

    input[type="range"]::-ms-fill-upper {
      visibility: visible;
      height: 12px;
      width: 12px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
    }
  }

  ${isMobile && mobileCss}
`;
