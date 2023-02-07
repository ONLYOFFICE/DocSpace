import styled, { css } from "styled-components";
import { CalendarStyle } from "@docspace/components/calendar/styled-calendar";

const getDefaultStyles = ({ $currentColorScheme, color }) =>
  $currentColorScheme &&
  css`
    .calendar-month_selected-day {
      background-color: ${color ? color : $currentColorScheme.main.accent};
      color: ${$currentColorScheme.text.accent};
      &:hover {
        background-color: ${color ? color : $currentColorScheme.main.accent};
        color: ${$currentColorScheme.text.accent};
      }
    }
  `;

export default styled(CalendarStyle)(getDefaultStyles);
