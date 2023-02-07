import styled from "styled-components";
import Base from "../../themes/base";

export const Title = styled.h2`
  font-family: "Open Sans", sans-serif, Arial;
  font-weight: 700;
  font-size: 21px;
  line-height: 28px;
  color: ${(props) => props.theme.calendar.titleColor};
  border-bottom: 1px dashed transparent;
  margin: 0;
  display: inline-block;

  :hover {
    border-bottom: ${(props) => (props.disabled ? "none" : "1px dashed")};
    border-color: ${(props) => props.theme.calendar.titleColor};
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
  }
`;

Title.defaultProps = { theme: Base };
