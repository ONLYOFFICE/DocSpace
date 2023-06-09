import styled, { css } from "styled-components";

export const Hint = styled.div`
  box-sizing: border-box;
  padding: 8px 12px;
  background: ${(props) => (props.backgroundColor ? props.backgroundColor : "#f8f7bf")};
  color: ${(props) => (props.color ? props.color : "initial")};
  border-radius: 6px;
  font-family: "Open Sans";
  font-style: normal;
  font-weight: 400;
  font-size: 12px;
  line-height: 16px;

  position: relative;
  z-index: 3;

  ${(props) =>
    props.isTooltip &&
    css`
      position: absolute;
      z-index: 2;

      width: 320px;
    `}
`;
