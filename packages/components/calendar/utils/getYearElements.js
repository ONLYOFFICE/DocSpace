import React from "react";
 import moment from "moment";

import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

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
    <SecondaryDateItem
      big
      key={year}
      onClick={() => onClick(year)}
      disabled={
        moment(year.toString()).endOf("year").endOf("month") < minDate ||
        moment(year.toString()) > maxDate
      }
    >
      {year}
    </SecondaryDateItem>
  ));

  for (let i = 1; i < 11; i++) {
    yearElements[i] = (
      <DateItem
        big
        key={years[i]}
        onClick={() => onClick(years[i])}
        disabled={
          moment(years[i].toString()).endOf("year").endOf("month") < minDate ||
          moment(years[i].toString()) > maxDate
        }
      >
        {years[i]}
      </DateItem>
    );
  }

  const currentYearIndex = years.indexOf(moment().year());
  const selectedYearIndex = years.indexOf(moment(selectedDate).year());
  if (selectedYearIndex !== -1) {
    yearElements[selectedYearIndex] = (
      <DateItem
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
      </DateItem>
    );
  }
  if (currentYearIndex !== -1) {
    yearElements[currentYearIndex] = (
      <CurrentDateItem
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
      </CurrentDateItem>
    );
  }

  return yearElements;
};
