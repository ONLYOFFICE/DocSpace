import React from "react";
import moment from "moment";

import { getCalendarDays } from "./getCalendarDays";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

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
      <ColorTheme
        className="day"
        themeId={ThemeType.DateItem}
        isSecondary
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </ColorTheme>
    )),
    currentMonthDays: calendarDays.currentMonthDays.map((day) => (
      <ColorTheme
        className="day"
        themeId={ThemeType.DateItem}
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </ColorTheme>
    )),
    nextMonthDays: calendarDays.nextMonthDays.map((day) => (
      <ColorTheme
        className="day"
        themeId={ThemeType.DateItem}
        isSecondary
        key={day.key}
        onClick={() => onClick(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
      >
        {day.value}
      </ColorTheme>
    )),
  };

  const currentDate = moment().format("YYYY-MM") + "-" + moment().date();
  const selectedDateFormated =
    moment(selectedDate).format("YYYY-MM") + "-" + moment(selectedDate).date();

  for (const key in calendarDays) {
    calendarDays[key].forEach((day, index) => {
      if (day.key === currentDate) {
        monthDays[key][index] = (
          <ColorTheme
            className="day"
            themeId={ThemeType.DateItem}
            isCurrent
            key={day.key}
            onClick={() => onClick(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
          >
            {day.value}
          </ColorTheme>
        );
      } else if (day.key === selectedDateFormated) {
        monthDays[key][index] = (
          <ColorTheme
            className="day"
            themeId={ThemeType.DateItem}
            key={day.key}
            focused
            onClick={() => onClick(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
          >
            {day.value}
          </ColorTheme>
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
