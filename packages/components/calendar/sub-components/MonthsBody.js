import { MonthsContainer } from "../styled-components/MonthsContainer";
import { getCalendarMonths, getMonthElements } from "../utils";

export const MonthsBody = ({ selectedDate, setSelectedDate }) => {
  const months = getCalendarMonths(selectedDate);
  const monthsElements = getMonthElements(months, setSelectedDate);

  return <MonthsContainer>{monthsElements}</MonthsContainer>;
};
