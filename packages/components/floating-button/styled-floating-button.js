import Base from "@docspace/components/themes/base";
import styled, { keyframes, css } from "styled-components";
import { desktop, tablet } from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

const StyledCircleWrap = styled.div`
  position: relative;
  z-index: 500;
  width: 48px;
  height: 48px;
  background: ${(props) =>
    props.color ? props.color : props.theme.floatingButton.backgroundColor};
  border-radius: 50%;
  cursor: pointer;
  box-shadow: ${(props) => props.theme.floatingButton.boxShadow};
`;

StyledCircleWrap.defaultProps = { theme: Base };

const StyledFloatingButtonWrapper = styled.div`
  @media ${desktop} {
    position: absolute;
    z-index: 300;
    right: 0;
    bottom: 0;

    ${!isMobile &&
    css`
      width: 100px;
      height: 70px;
    `}
  }

  .layout-progress-bar_close-icon {
    display: none;
    position: absolute;
    cursor: pointer;
    right: 77px;
    bottom: 33px;
  }
  &:hover {
    .layout-progress-bar_close-icon {
      display: block;
    }
    @media ${tablet} {
      .layout-progress-bar_close-icon {
        display: none;
      }
    }
  }

  @media ${tablet} {
    .layout-progress-bar_close-icon {
      display: none;
    }
  }
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
    background-color: ${(props) =>
      !props.displayProgress
        ? "transparent !important"
        : props.theme.floatingButton.color};
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
  background: ${(props) =>
    props.color ? props.color : props.theme.floatingButton.backgroundColor};
  text-align: center;
  margin: 5px;
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
`;

StyledFloatingButton.defaultProps = { theme: Base };

const IconBox = styled.div`
  // padding-top: 12px;
  display: flex;
  align-items: center;
  justify-content: center;

  svg {
    path {
      fill: ${(props) => props.theme.floatingButton.fill};
    }
  }
`;

IconBox.defaultProps = { theme: Base };

const StyledAlertIcon = styled.div`
  position: absolute;
  width: 12px;
  height: 12px;
  left: 20px;
  top: 0px;

  svg {
    circle {
      fill: ${(props) => props.theme.floatingButton.alert.fill};
    }
    path {
      fill: ${(props) => props.theme.floatingButton.alert.path};
    }
  }
`;

export {
  StyledFloatingButtonWrapper,
  StyledCircle,
  StyledCircleWrap,
  StyledFloatingButton,
  StyledAlertIcon,
  IconBox,
};
