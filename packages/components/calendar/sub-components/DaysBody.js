import { DaysContainer } from "../styled-components";

import { getDayElements, getWeekdayElements } from "../utils";

export const DaysBody = ({ observedDate, setSelectedDate, selectedDate }) => {
  const daysElements = getDayElements(
    observedDate,
    selectedDate,
    setSelectedDate
  );
  const weekdayElements = getWeekdayElements();

  return (
    <DaysContainer>
      {weekdayElements} {daysElements}
    </DaysContainer>
  );
};
