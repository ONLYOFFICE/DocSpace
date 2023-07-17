import React from "react";
import moment from "moment";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const onDateClick = (year, setObservedDate, setSelectedScene) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().set({ year: year })
  );
  setSelectedScene((prevSelectedScene) => prevSelectedScene - 1);
};

export const getYearElements = (
  years,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate
) => {
  const onClick = (year) =>
    onDateClick(year, setObservedDate, setSelectedScene);

  const yearElements = years.map((year) => (
    <ColorTheme
      className="year"
      themeId={ThemeType.DateItem}
      isSecondary
      big
      key={year}
      onClick={() => onClick(year)}
      disabled={
        moment(year.toString()).endOf("year").endOf("month") < minDate ||
        moment(year.toString()) > maxDate
      }
    >
      {year}
    </ColorTheme>
  ));

  for (let i = 1; i < 11; i++) {
    yearElements[i] = (
      <ColorTheme
        className="year"
        themeId={ThemeType.DateItem}
        big
        key={years[i]}
        onClick={() => onClick(years[i])}
        disabled={
          moment(years[i].toString()).endOf("year").endOf("month") < minDate ||
          moment(years[i].toString()) > maxDate
        }
      >
        {years[i]}
      </ColorTheme>
    );
  }

  const currentYearIndex = years.indexOf(moment().year());
  const selectedYearIndex = years.indexOf(moment(selectedDate).year());
  if (selectedYearIndex !== -1) {
    yearElements[selectedYearIndex] = (
      <ColorTheme
        className="year"
        themeId={ThemeType.DateItem}
        big
        focused
        key={years[selectedYearIndex]}
        onClick={() => onClick(years[selectedYearIndex])}
        disabled={
          moment(years[selectedYearIndex].toString())
            .endOf("year")
            .endOf("month") < minDate ||
          moment(years[selectedYearIndex].toString()) > maxDate
        }
      >
        {years[selectedYearIndex]}
      </ColorTheme>
    );
  }
  if (currentYearIndex !== -1) {
    yearElements[currentYearIndex] = (
      <ColorTheme
        className="year"
        themeId={ThemeType.DateItem}
        isCurrent
        big
        key={years[currentYearIndex]}
        onClick={() => onClick(years[currentYearIndex])}
        disabled={
          moment(years[currentYearIndex].toString())
            .endOf("year")
            .endOf("month") < minDate ||
          moment(years[currentYearIndex].toString()) > maxDate
        }
      >
        {years[currentYearIndex]}
      </ColorTheme>
    );
  }

  return yearElements;
};
