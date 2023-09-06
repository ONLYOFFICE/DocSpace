import styled, { css } from "styled-components";

export const SpeedControlWrapper = styled.div`
  position: relative;

  display: flex;
  justify-content: center;
  align-items: center;
  width: 48px;
  height: 48px;

  &:hover {
    cursor: pointer;
  }

  svg {
    path {
      fill: #fff;
    }
  }

  rect {
    stroke: #fff;
  }
`;

export const DropDown = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;

  height: 120px;
  width: 48px;

  padding: 4px 0px;

  position: absolute;
  bottom: 48px;
  z-index: 50;

  color: #fff;
  background: #333;
  text-align: center;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          border-radius: 7px 0px 0px 7px;
        `
      : css`
          border-radius: 7px 7px 0px 0px;
        `}
`;

export const DropDownItem = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  height: 30px;
  width: 48px;
  &:hover {
    cursor: pointer;
    background: #222;
  }
`;

export const ToastSpeed = styled.div`
  position: fixed;

  top: 50%;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          right: 50%;
        `
      : css`
          left: 50%;
        `}

  display: flex;
  justify-content: center;
  align-items: center;

  width: 72px;
  height: 56px;

  border-radius: 9px;
  visibility: visible;

  transform: translate(-50%, -50%);
  background-color: rgba(51, 51, 51, 0.65);

  svg {
    width: 46px;
    height: 46px;
    path {
      fill: #fff;
    }
  }

  rect {
    stroke: #fff;
  }
`;
