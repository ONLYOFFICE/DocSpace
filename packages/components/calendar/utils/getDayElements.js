import React from "react";
import moment from "moment";

import { getCalendarDays } from "./getCalendarDays";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

export const getDayElements = (
  observedDate,
  selectedDate,
  handleDateChange,
  minDate,
  maxDate,
  isMobile
) => {
  const dateFormat = "YYYY-MM-D";

  const calendarDays = getCalendarDays(observedDate);

  const monthDays = {
    prevMonthDays: calendarDays.prevMonthDays.map((day) => (
      <ColorTheme
        className="day"
        themeId={ThemeType.DateItem}
        isSecondary
        key={day.key}
        onClick={() => handleDateChange(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
        isMobile={isMobile}
      >
        {day.value}
      </ColorTheme>
    )),
    currentMonthDays: calendarDays.currentMonthDays.map((day) => (
      <ColorTheme
        className="day"
        themeId={ThemeType.DateItem}
        key={day.key}
        onClick={() => handleDateChange(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
        isMobile={isMobile}
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
        onClick={() => handleDateChange(moment(day.key, dateFormat))}
        disabled={
          moment(day.key, dateFormat) < minDate ||
          moment(day.key, dateFormat) > maxDate
        }
        isMobile={isMobile}
      >
        {day.value}
      </ColorTheme>
    )),
  };

  const currentDate = moment().format("YYYY-MM-") + moment().format("D");
  const selectedDateFormated =
    moment(selectedDate).format("YYYY-MM-") + moment(selectedDate).format("D");

  for (const key in calendarDays) {
    calendarDays[key].forEach((day, index) => {
      if (day.key === currentDate) {
        monthDays[key][index] = (
          <ColorTheme
            className="day"
            themeId={ThemeType.DateItem}
            isCurrent
            key={day.key}
            onClick={() => handleDateChange(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
            isMobile={isMobile}
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
            onClick={() => handleDateChange(moment(day.key, dateFormat))}
            disabled={
              moment(day.key, dateFormat) < minDate ||
              moment(day.key, dateFormat) > maxDate
            }
            isMobile={isMobile}
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
