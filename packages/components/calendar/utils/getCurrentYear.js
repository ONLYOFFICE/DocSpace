// takes array of years ['2012', '2013' ...], openToDate Date obj
// returns current year

export const getCurrentYear = (arrayYears, openToDate) => {
  const openToDateYear = openToDate.getFullYear();
  let currentYear = arrayYears.find((x) => x.key == openToDateYear);
  if (!currentYear) {
    const newDate = this.props.minDate.getFullYear();
    currentYear = { key: newDate, label: `${newDate}` };
  }
  return currentYear;
};
