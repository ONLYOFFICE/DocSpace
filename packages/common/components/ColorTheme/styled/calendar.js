import styled, { css } from "styled-components";
import { CalendarStyle } from "@docspace/components/calendar/styled-calendar";

const getDefaultStyles = ({ currentColorScheme, color }) => css`
  .calendar-month_selected-day {
    background-color: ${color ? color : currentColorScheme.accentColor};
    &:hover {
      background-color: ${color ? color : currentColorScheme.accentColor};
    }
  }
`;

export default styled(CalendarStyle)(getDefaultStyles);
