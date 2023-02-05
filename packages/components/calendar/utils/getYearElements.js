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
  selectedDate
) => {
  const onClick = (year) =>
    onDateClick(year, setObservedDate, setSelectedScene);

  const yearElements = years.map((year) => (
    <SecondaryDateItem big key={year} onClick={() => onClick(year)}>
      {year}
    </SecondaryDateItem>
  ));

  for (let i = 1; i < 11; i++) {
    yearElements[i] = (
      <DateItem big key={years[i]} onClick={() => onClick(years[i])}>
        {years[i]}
      </DateItem>
    );
  }

  const currentYearIndex = years.indexOf(moment().year());
  const selectedYearIndex = years.indexOf(selectedDate.year());
  if (selectedYearIndex !== -1) {
    yearElements[selectedYearIndex] = (
      <DateItem
        big
        focused
        key={years[selectedYearIndex]}
        onClick={() => onClick(years[selectedYearIndex])}
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
      >
        {years[currentYearIndex]}
      </CurrentDateItem>
    );
  }

  return yearElements;
};
