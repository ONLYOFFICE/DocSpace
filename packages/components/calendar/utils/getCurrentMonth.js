// takes array of months, openToDate Date obj
// returns current month

export const getCurrentMonth = (months, openToDate) => {
  const openToDateMonth = openToDate.getMonth();
  let selectedMonth = months.find((x) => x.key == openToDateMonth);

  if (selectedMonth.disabled === true) {
    selectedMonth = months.find((x) => x.disabled === false);
  }
  return selectedMonth;
};