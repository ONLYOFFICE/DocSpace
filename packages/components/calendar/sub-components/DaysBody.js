import { DaysContainer } from "../styled-components";

import { getDayElements, getWeekdayElements } from "../utils";

export const DaysBody = ({ selectedDate, setSelectedDate }) => {
  const daysElements = getDayElements(selectedDate, setSelectedDate);
  const weekdayElements = getWeekdayElements();

  return (
    <DaysContainer>
      {weekdayElements} {daysElements}
    </DaysContainer>
  );
};
