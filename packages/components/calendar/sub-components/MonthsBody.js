import { MonthsContainer } from "../styled-components/MonthsContainer";
import { getCalendarMonths, getMonthElements } from "../utils";

export const MonthsBody = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  selectedDate
}) => {
  const months = getCalendarMonths(observedDate);
  const monthsElements = getMonthElements(
    months,
    setObservedDate,
    setSelectedScene,
    selectedDate
  );

  return <MonthsContainer>{monthsElements}</MonthsContainer>;
};
