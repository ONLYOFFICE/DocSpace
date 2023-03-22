import { tablet } from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";
import styled, { css } from "styled-components";

export const PlayerVolumeControlWrapper = styled.div`
  display: flex;
  align-items: center;
  margin-left: 10px;
`;

export const IconWrapper = styled.div`
  display: flex;
  align-items: center;
  cursor: pointer;
`;

const mobilecss = css`
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

export const VolumeWrapper = styled.div`
  width: 123px;
  height: 28px;
  display: flex;
  align-items: center;
  padding-left: 9px;

  input {
    margin-right: 15px;
    width: 80%;
    height: 4px;

    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;

    border-radius: 5px;

    background: rgba(255, 255, 255, 0.3);
    background-image: linear-gradient(#fff, #fff);
    background-repeat: no-repeat;

    &:hover {
      cursor: pointer;
    }

    @media ${tablet} {
      width: 63%;
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
      height: 10px;
      width: 10px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
    }

    input[type="range"]::-moz-range-thumb {
      visibility: visible;
      height: 10px;
      width: 10px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
      border: none;
    }

    input[type="range"]::-ms-fill-upper {
      visibility: visible;
      height: 10px;
      width: 10px;
      opacity: 1 !important;
      border-radius: 50%;
      cursor: pointer;
    }
  }

  ${isMobile && mobilecss}
`;
