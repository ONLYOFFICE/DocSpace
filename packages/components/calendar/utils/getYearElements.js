import moment from "moment";

import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

const onDateClick = (year, setSelectedDate) => {
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().set({ year: year })
  );
};

export const getYearElements = (years, setSelectedDate) => {
  const yearElements = years.map((year) => (
    <SecondaryDateItem
      big
      key={year}
      onClick={() => onDateClick(year, setSelectedDate)}
    >
      {year}
    </SecondaryDateItem>
  ));

  for (let i = 1; i < 11; i++) {
    yearElements[i] = (
      <DateItem
        big
        key={years[i]}
        onClick={() => onDateClick(years[i], setSelectedDate)}
      >
        {years[i]}
      </DateItem>
    );
  }

  const currentYearIndex = years.indexOf(moment().year());
  if (currentYearIndex !== -1) {
    yearElements[currentYearIndex] = (
      <CurrentDateItem
        big
        key={years[currentYearIndex]}
        onClick={() => onDateClick(years[currentYearIndex], setSelectedDate)}
      >
        {years[currentYearIndex]}
      </CurrentDateItem>
    );
  }

  return yearElements;
};
