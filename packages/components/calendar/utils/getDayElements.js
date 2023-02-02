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

export const getDayElements = (selectedDate, setSelectedDate) => {
  const calendarDays = getCalendarDays(selectedDate);
  const monthDays = {
    prevMonthDays: calendarDays.prevMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onDateClick(setSelectedDate, moment(day.key))}
      >
        {day.value}
      </SecondaryDateItem>
    )),
    currentMonthDays: calendarDays.currentMonthDays.map((day) => (
      <DateItem
        key={day.key}
        onClick={() => onDateClick(setSelectedDate, moment(day.key))}
      >
        {day.value}
      </DateItem>
    )),
    nextMonthDays: calendarDays.nextMonthDays.map((day) => (
      <SecondaryDateItem
        key={day.key}
        onClick={() => onDateClick(setSelectedDate, moment(day.key))}
      >
        {day.value}
      </SecondaryDateItem>
    )),
  };

  const currentDate = moment().format("YYYY-MM") + "-" + moment().date();

  for (const key in calendarDays) {
    calendarDays[key].forEach((day, index) => {
      if (day.key === currentDate) {
        monthDays[key][index] = (
          <CurrentDateItem
            key={day.key}
            onClick={() => onDateClick(setSelectedDate, moment(day.key))}
          >
            {day.value}
          </CurrentDateItem>
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
