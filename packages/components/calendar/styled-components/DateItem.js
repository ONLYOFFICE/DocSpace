import styled from "styled-components";
import Base from "../../themes/base";

export const DateItem = styled.button`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 16px;
  border-radius: 50%;
  color: ${(props) =>
    props.disabled
      ? props.theme.calendar.disabledColor
      : props.focused
      ? "#4781D1"
      : props.theme.calendar.color};

  border: 2px solid;
  border-color: ${(props) => (props.focused ? "#4781D1" : "transparent")};
  background-color: transparent;

  width: ${(props) => (props.big ? "60px" : "40px")};
  height: ${(props) => (props.big ? "60px" : "40px")};

  display: inline-flex;
  justify-content: center;
  align-items: center;

  :hover {
    cursor: ${(props) => (props.disabled ? "default" : "pointer")};
    background: ${(props) =>
      props.disabled
        ? "transparent"
        : props.theme.calendar.onHoverBackground};
  }
`;
DateItem.defaultProps = { theme: Base };
