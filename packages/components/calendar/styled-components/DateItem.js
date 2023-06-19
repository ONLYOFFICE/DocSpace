import styled, { css } from "styled-components";
import Base from "../../themes/base";

export const DateItem = styled.button`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 16px;
  border-radius: 50%;

  border: 2px solid;
  background-color: transparent;

  width: ${(props) => (props.big ? "60px" : "40px")};
  height: ${(props) => (props.big ? "60px" : "40px")};

  display: inline-flex;
  justify-content: center;
  align-items: center;

  :hover {
    cursor: ${(props) => (props.disabled ? "default" : "pointer")};
    background: ${(props) =>
      props.disabled ? "transparent" : props.theme.calendar.onHoverBackground};
  }

  ${(props) =>
    props.isCurrent &&
    css`
      color: white !important;

      :hover {
        color: white !important;
      }

      :focus {
        color: white !important;
      }
    `}
  ${(props) =>
    props.isSecondary &&
    css`
      color: ${(props) =>
        props.disabled
          ? props.theme.calendar.disabledColor
          : props.theme.calendar.pastColor} !important;

      :hover {
        cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
        color: ${(props) =>
          props.disabled
            ? props.theme.calendar.disabledColor
            : props.theme.calendar.pastColor} !important;
      }
    `}
`;
DateItem.defaultProps = { theme: Base };
