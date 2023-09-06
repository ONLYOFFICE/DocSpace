import styled, { css } from "styled-components";

type StyledSwitchToolbarProps = {
  left?: boolean;
  isPdfFIle?: boolean;
};

const StyledSwitchToolbar = styled.div<StyledSwitchToolbarProps>`
  height: 100%;
  z-index: 306;
  position: fixed;
  width: 73px;
  background: inherit;
  display: block;
  opacity: 0;
  transition: all 0.3s;

  ${props =>
    props.left
      ? props.theme.interfaceDirection === "rtl"
        ? "right: 0"
        : "left: 0"
      : props.isPdfFIle
      ? props.theme.interfaceDirection === "rtl"
        ? "left: 20px"
        : "right: 20px"
      : props.theme.interfaceDirection === "rtl"
      ? "left: 0"
      : "right: 0"};

  &:hover {
    cursor: pointer;
    opacity: 1;
  }
`;

export default StyledSwitchToolbar;
