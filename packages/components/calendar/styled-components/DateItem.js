import styled from "styled-components";
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
`;
DateItem.defaultProps = { theme: Base };
