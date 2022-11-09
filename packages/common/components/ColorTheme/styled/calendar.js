import styled, { css } from "styled-components";
import { CalendarStyle } from "@docspace/components/calendar/styled-calendar";

const getDefaultStyles = ({ $currentColorScheme, color }) =>
  $currentColorScheme &&
  css`
    .calendar-month_selected-day {
      background-color: ${color ? color : $currentColorScheme.accentColor};
      color: ${$currentColorScheme.textColor};
      &:hover {
        background-color: ${color ? color : $currentColorScheme.accentColor};
        color: ${$currentColorScheme.textColor};
      }
    }
  `;

export default styled(CalendarStyle)(getDefaultStyles);
