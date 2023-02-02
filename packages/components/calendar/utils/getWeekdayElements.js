import { Weekday } from "../styled-components";

export const getWeekdayElements = () => {
  const weekdays = ["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"];
  return weekdays.map((day) => <Weekday key={day}>{day}</Weekday>);
};
