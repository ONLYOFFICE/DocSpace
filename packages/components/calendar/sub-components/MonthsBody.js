import { MonthsContainer } from "../styled-components/MonthsContainer";
import { getCalendarMonths, getMonthElements } from "../utils";

export const MonthsBody = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate
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

  return <MonthsContainer>{monthsElements}</MonthsContainer>;
};
