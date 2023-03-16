export const getCalendarYears = (observedDate) => {
  const years = [];
  const selectedYear = observedDate.year();
  const firstYear = selectedYear - (selectedYear % 10) - 1;

  for (let i = firstYear; i <= firstYear + 15; i++) {
    years.push(i);
  }

  return years;
};
