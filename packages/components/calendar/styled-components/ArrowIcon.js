import styled, { css } from "styled-components";
import Base from "../../themes/base";

export const ArrowIcon = styled.span`
  position: absolute;
  border-left: 2px solid;
  border-bottom: 2px solid;
  width: 5px;
  height: 5px;

  ${(props) =>
    props.next &&
    css`
      transform: rotate(-45deg);
      top: 11.5px;
      left: 12.5px;
    `}
  ${(props) =>
    props.previous &&
    css`
      transform: rotate(135deg);
      top: 14px;
      left: 12.5px;
    `}
`;

ArrowIcon.defaultProps = { theme: Base };
