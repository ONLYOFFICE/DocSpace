import { Weekday } from "../styled-components";
import moment from "moment";

export const getWeekdayElements = () => {
  const weekdays = moment.weekdaysMin(true);
  return weekdays.map((day) => <Weekday key={day}>{day}</Weekday>);
};
