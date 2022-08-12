import styled, { css, keyframes } from "styled-components";
import globalColors from "@appserver/components/utils/globalColors";
import { Base } from "@appserver/components/themes";

const backgroundColor = "none";

const StyledCircleWrap = styled.div`
  width: 16px;
  height: 16px;
  background: ${backgroundColor};
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
    width: 16px;
    height: 16px;
    position: absolute;
    border-radius: 50%;
  }

  ${(props) =>
    props.percent === 0 || (props.isAnimation && props.inConversion)
      ? css`
          .circle__fill {
            animation: ${rotate360} 2s linear infinite;
            transform: translate(0);
          }
        `
      : css`
          .circle__mask {
            clip: rect(0px, 16px, 16px, 8px);
          }

          .circle__fill {
            animation: fill-rotate ease-in-out none;
            transform: rotate(${(props) => props.percent * 1.8}deg);
          }
        `}

  .circle__mask .circle__fill {
    clip: rect(0px, 8px, 16px, 0px);
    background-color: ${(props) =>
      props.theme.filesPanels.upload.loadingButton.color};
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

StyledCircleWrap.defaultProps = { theme: Base };

const StyledLoadingButton = styled.div`
  width: 12px;
  height: 12px;
  border-radius: 50%;
  text-align: center;
  line-height: 12px;
  background: ${(props) =>
    props.theme.filesPanels.upload.loadingButton.background};
  position: absolute;
  margin: 2px;
  color: ${(props) => props.theme.filesPanels.upload.loadingButton.color};
  font-size: 16px;
  font-weight: bold;
`;

StyledLoadingButton.defaultProps = { theme: Base };

export { StyledCircle, StyledCircleWrap, StyledLoadingButton };
