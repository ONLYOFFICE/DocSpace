import styled from "styled-components";
import Base from "../../themes/base";

export const Weekday = styled.span`
  pointer-events: none;
  font-family: "Open Sans";
  font-weight: 400;
  font-size: ${(props) => (props.isMobile ? "16px" : "13px")};
  line-height: 16px;

  color: ${(props) => props.theme.calendar.weekdayColor};
  width: ${(props) => (props.isMobile ? "40px" : "30px")};

  text-align: center;
  padding: 10.7px 0;
`;
Weekday.defaultProps = { theme: Base };
