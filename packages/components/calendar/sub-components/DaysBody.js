import { DaysContainer } from "../styled-components";

import { getDayElements, getWeekdayElements } from "../utils";

export const DaysBody = ({
  observedDate,
  handleDateChange,
  selectedDate,
  minDate,
  maxDate,
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
    <DaysContainer>
      {weekdayElements} {daysElements}
    </DaysContainer>
  );
};
