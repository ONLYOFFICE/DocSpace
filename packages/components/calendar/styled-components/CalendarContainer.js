import styled, { css } from "styled-components";

export const CalendarContainer = styled.div`
  ${(props) =>
    !props.isMobile &&
    css`
      width: 306px;
      height: 276px;
    `}

  box-sizing: border-box;

  display: grid;
  row-gap: ${(props) =>
    props.big
      ? props.isMobile
        ? "26.7px"
        : "10px"
      : props.isMobile
      ? "9px"
      : "0"};
  column-gap: ${(props) =>
    props.big
      ? props.isMobile
        ? "8%"
        : "31.33px"
      : props.isMobile
      ? "2%"
      : "14px"};
  grid-template-columns: ${(props) =>
    props.big ? "repeat(4, 1fr)" : "repeat(7, 1fr)"};
  box-sizing: border-box;
  padding: ${(props) => (props.big ? "14px 6px 6px 6px" : "0 6px")};
`;
