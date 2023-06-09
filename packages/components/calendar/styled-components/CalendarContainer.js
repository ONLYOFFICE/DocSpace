import styled from "styled-components";

export const CalendarContainer = styled.div`
  display: grid;
  row-gap: ${(props) => (props.big ? "26.7px" : "9px")};
  column-gap: ${(props) =>
    props.big
      ? props.isMobile
        ? "8%"
        : "41.3px"
      : props.isMobile
      ? "2%"
      : "14px"};
  grid-template-columns: ${(props) =>
    props.big ? "repeat(4, 1fr)" : "repeat(7, 1fr)"};
  box-sizing: border-box;
  padding: ${(props) => (props.big ? "14px 6px 6px 6px" : "0 6px")};
`;
