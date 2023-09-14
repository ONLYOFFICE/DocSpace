import React from "react";
import moment from "moment";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

export const getYearElements = (
  years,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate,
  isMobile
) => {
  const onDateClick = (year) => {
    setObservedDate((prevObservedDate) =>
      moment(
        `${moment(year, "YYYY").format("YYYY")}-${prevObservedDate.format(
          "MM-DD"
        )}`,
        "YYYY-MM-DD"
      )
    );
    setSelectedScene((prevSelectedScene) => prevSelectedScene - 1);
  };

  const yearElements = years.map((year) => (
    <ColorTheme
      className="year"
      themeId={ThemeType.DateItem}
      isSecondary
      big
      key={year}
      onClick={() => onDateClick(year)}
      disabled={
        moment(year.toString()).endOf("year").endOf("month") < minDate ||
        moment(year.toString()) > maxDate
      }
      isMobile={isMobile}
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
        onClick={() => onDateClick(years[i])}
        disabled={
          moment(years[i].toString()).endOf("year").endOf("month") < minDate ||
          moment(years[i].toString()) > maxDate
        }
        isMobile={isMobile}
      >
        {years[i]}
      </ColorTheme>
    );
  }

  const currentYearIndex = years.indexOf(moment().format("YYYY"));
  const selectedYearIndex = years.indexOf(moment(selectedDate).format("YYYY"));
  if (selectedYearIndex !== -1) {
    yearElements[selectedYearIndex] = (
      <ColorTheme
        className="year"
        themeId={ThemeType.DateItem}
        big
        focused
        key={years[selectedYearIndex]}
        onClick={() => onDateClick(years[selectedYearIndex])}
        disabled={
          moment(years[selectedYearIndex].toString())
            .endOf("year")
            .endOf("month") < minDate ||
          moment(years[selectedYearIndex].toString()) > maxDate
        }
        isMobile={isMobile}
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
        onClick={() => onDateClick(years[currentYearIndex])}
        disabled={
          moment(years[currentYearIndex].toString())
            .endOf("year")
            .endOf("month") < minDate ||
          moment(years[currentYearIndex].toString()) > maxDate
        }
        isMobile={isMobile}
      >
        {years[currentYearIndex]}
      </ColorTheme>
    );
  }

  return yearElements;
};
