import styled, { css } from "styled-components";

const WrapperPlayerBigPlayButton = styled.div`
  position: absolute;
  top: 50%;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          right: 50%;
        `
      : css`
          left: 50%;
        `}
  transform: translate(-50%, -50%);
  cursor: pointer;
`;

export default WrapperPlayerBigPlayButton;
