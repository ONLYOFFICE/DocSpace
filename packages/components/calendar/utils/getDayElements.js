import moment from "moment";

import { getCalendarDays } from "./getCalendarDays";
import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

const onDateClick = (setSelectedDate, newDate) => {
  setSelectedDate(moment(newDate));
};

export const getDayElements = (
  observedDate,
  selectedDate,
  setSelectedDate,
  minDate,
  maxDate
) => {
  const onClick = (newDate) => onDateClick(setSelectedDate, newDate);

  const calendarDays = getCalendarDays(observedDate);
  const monthDays = {
    prevMonthDays: calendarDays.prevMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onClick(moment(day.key))}
        disabled={moment(day.key) < minDate || moment(day.key) > maxDate}
      >
        {day.value}
      </SecondaryDateItem>
    )),
    currentMonthDays: calendarDays.currentMonthDays.map((day) => (
      <DateItem
        key={day.key}
        onClick={() => onClick(moment(day.key))}
        disabled={moment(day.key) < minDate || moment(day.key) > maxDate}
      >
        {day.value}
      </DateItem>
    )),
    nextMonthDays: calendarDays.nextMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onClick(moment(day.key))}
        disabled={moment(day.key) < minDate || moment(day.key) > maxDate}
      >
        {day.value}
      </SecondaryDateItem>
    )),
  };

  const currentDate = moment().format("YYYY-MM") + "-" + moment().date();
  const selectedDateFormated =
    selectedDate.format("YYYY-MM") + "-" + selectedDate.date();

  for (const key in calendarDays) {
    calendarDays[key].forEach((day, index) => {
      if (day.key === currentDate) {
        monthDays[key][index] = (
          <CurrentDateItem
            key={day.key}
            onClick={() => onClick(moment(day.key))}
            disabled={moment(day.key) < minDate || moment(day.key) > maxDate}
          >
            {day.value}
          </CurrentDateItem>
        );
      } else if (day.key === selectedDateFormated) {
        monthDays[key][index] = (
          <DateItem
            key={day.key}
            focused
            onClick={() => onClick(moment(day.key))}
            disabled={moment(day.key) < minDate || moment(day.key) > maxDate}
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
