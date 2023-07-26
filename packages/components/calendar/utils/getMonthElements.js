import React from "react";
import moment from "moment";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const onDateClick = (dateString, setObservedDate, setSelectedScene) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().set({
      year: dateString.substring(0, 4),
      month: dateString.substring(5) - 1,
    })
  );
  setSelectedScene((prevSelectedScene) => prevSelectedScene - 1);
};

export const getMonthElements = (
  months,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate
) => {
  const onClick = (dateString) =>
    onDateClick(dateString, setObservedDate, setSelectedScene);

  const dateFormat = "YYYY-M";

  const monthsElements = months.map((month) => (
    <ColorTheme
      className="month"
      themeId={ThemeType.DateItem}
      big
      key={month.key}
      onClick={() => onClick(month.key)}
      disabled={
        moment(month.key, dateFormat).endOf("month") < minDate ||
        moment(month.key, dateFormat).startOf("month") > maxDate
      }
    >
      {month.value}
    </ColorTheme>
  ));
  for (let i = 12; i < 16; i++) {
    monthsElements[i] = (
      <ColorTheme
        className="month"
        themeId={ThemeType.DateItem}
        isSecodary
        big
        key={months[i].key}
        onClick={() => onClick(months[i].key)}
        disabled={
          moment(months[i].key, dateFormat).endOf("month") < minDate ||
          moment(months[i].key, dateFormat).startOf("month") > maxDate
        }
      >
        {months[i].value}
      </ColorTheme>
    );
  }

  const currentDate = `${moment().year()}-${moment().format("M")}`;
  const formattedDate = `${moment(selectedDate).year()}-${moment(
    selectedDate
  ).format("M")}`;

  months.forEach((month, index) => {
    if (month.key === currentDate) {
      monthsElements[index] = (
        <ColorTheme
          className="month"
          themeId={ThemeType.DateItem}
          isCurrent
          big
          key={month.key}
          onClick={() => onClick(month.key)}
          disabled={
            moment(month.key, dateFormat).endOf("month") < minDate ||
            moment(month.key, dateFormat).startOf("month") > maxDate
          }
        >
          {month.value}
        </ColorTheme>
      );
    } else if (month.key === formattedDate) {
      monthsElements[index] = (
        <ColorTheme
          className="month"
          themeId={ThemeType.DateItem}
          big
          key={month.key}
          focused
          onClick={() => onClick(month.key)}
          disabled={
            moment(month.key, dateFormat).endOf("month") < minDate ||
            moment(month.key, dateFormat).startOf("month") > maxDate
          }
        >
          {month.value}
        </ColorTheme>
      );
    }
  });
  return monthsElements;
};
