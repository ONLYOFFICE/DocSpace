import React from "react";
import { CalendarContainer } from "../styled-components";

import { getDayElements, getWeekdayElements } from "../utils";

export const DaysBody = ({
  observedDate,
  handleDateChange,
  selectedDate,
  minDate,
  maxDate,
  isMobile,
}) => {
  const daysElements = getDayElements(
    observedDate,
    selectedDate,
    handleDateChange,
    minDate,
    maxDate
  );
  const weekdayElements = getWeekdayElements();

  return (
    <CalendarContainer isMobile={isMobile}>
      {weekdayElements} {daysElements}
    </CalendarContainer>
  );
};
