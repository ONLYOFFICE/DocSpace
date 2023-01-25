import moment from "moment";

// takes Date object
// return first day in terms of weekday

export const getFirstMonthDay = (openToDate) => {
  const firstDay = moment(openToDate).locale("en").startOf("month").format("d");
  let day = firstDay - 1;
  if (day < 0) {
    day = 6;
  }
  return day;
};
