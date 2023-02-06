import { DaysContainer } from "../styled-components";

import { getDayElements, getWeekdayElements } from "../utils";

export const DaysBody = ({ observedDate, setSelectedDate, selectedDate, minDate, maxDate }) => {
  const daysElements = getDayElements(
    observedDate,
    selectedDate,
    setSelectedDate,
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
