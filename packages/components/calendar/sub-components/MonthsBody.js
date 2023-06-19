import React from "react";
import { CalendarContainer } from "../styled-components";
import { getCalendarMonths, getMonthElements } from "../utils";

export const MonthsBody = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate,
  isMobile,
}) => {
  const months = getCalendarMonths(observedDate);
  const monthsElements = getMonthElements(
    months,
    setObservedDate,
    setSelectedScene,
    selectedDate,
    minDate,
    maxDate
  );

  return (
    <CalendarContainer big isMobile={isMobile}>
      {monthsElements}
    </CalendarContainer>
  );
};
