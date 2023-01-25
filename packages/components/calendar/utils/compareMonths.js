import moment from "moment";

// takes 2 Date objects as input 
// => returns difference in months

export const compareMonths = (date1, date2) => {
  return moment(date1)
    .startOf("months")
    .diff(moment(date2).startOf("months"), "months");
};
