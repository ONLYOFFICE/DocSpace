import moment from "moment";

// takes 2 Date objects as input 
// => returns difference in days

export const compareDates = (date1, date2) => {
  return moment(date1)
    .startOf("day")
    .diff(moment(date2).startOf("day"), "days");
};
