import moment from "moment";

import { getCalendarDays } from "./getCalendarDays";
import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

const onDateClick = (handleDateChange, newDate) => {
  handleDateChange(moment(newDate));
};

export const getDayElements = (
  observedDate,
  selectedDate,
  handleDateChange,
  minDate,
  maxDate
) => {
  const onClick = (newDate) => onDateClick(handleDateChange, newDate);

  const dateFormat = "YYYY-MM-D";

  const calendarDays = getCalendarDays(observedDate);
  const monthDays = {
    prevMonthDays: calendarDays.prevMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </SecondaryDateItem>
    )),
    currentMonthDays: calendarDays.currentMonthDays.map((day) => (
      <DateItem
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </DateItem>
    )),
    nextMonthDays: calendarDays.nextMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </SecondaryDateItem>
    )),
  };

  const currentDate = moment().format("YYYY-MM") + "-" + moment().date();
  const selectedDateFormated =
    moment(selectedDate).format("YYYY-MM") + "-" + moment(selectedDate).date();

  for (const key in calendarDays) {
    calendarDays[key].forEach((day, index) => {
      if (day.key === currentDate) {
        monthDays[key][index] = (
          <CurrentDateItem
            key={day.key}
            onClick={() => onClick(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
          >
            {day.value}
          </CurrentDateItem>
        );
      } else if (day.key === selectedDateFormated) {
        monthDays[key][index] = (
          <DateItem
            key={day.key}
            focused
            onClick={() => onClick(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
          >
            {day.value}
          </DateItem>
        );
      }
    });
  }

  return [
    ...monthDays.prevMonthDays,
    ...monthDays.currentMonthDays,
    ...monthDays.nextMonthDays,
  ];
};
