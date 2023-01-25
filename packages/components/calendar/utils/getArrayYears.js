import moment from "moment";

// takes 2 Date objects: min and max dates
// => returns array of years covered between min and max dates
// returned object is {key: year number, label: year string}

export const getArrayYears = (minDate, maxDate) => {
  const minYear = minDate.getFullYear();
  const maxYear = maxDate.getFullYear();
  const yearList = [];

  let i = minYear;
  while (i <= maxYear) {
    let newDate = new Date(i, 0, 1);
    const label = moment(newDate).format("YYYY");
    const key = i;
    yearList.push({ key, label: label });
    i++;
  }
  return yearList.reverse();
};
