import React from "react";

import { CalendarContainer } from "../styled-components";
import { getCalendarYears, getYearElements } from "../utils";

export const YearsBody = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate,
  isMobile,
}) => {
  const years = getCalendarYears(observedDate);
  const yearElements = getYearElements(
    years,
    setObservedDate,
    setSelectedScene,
    selectedDate,
    minDate,
    maxDate
  );

  return (
    <CalendarContainer big isMobile={isMobile}>
      {yearElements}
    </CalendarContainer>
  );
};
