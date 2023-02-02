import moment from "moment";

export const getCalendarMonths = (selectedDate) => {
  // prettier-ignore
  const months = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec","Jan","Feb","Mar","Apr"];
  const monthsObjs = months.map((month) => ({
    key: `${selectedDate.year()}-${moment().month(month).format("M")}`,
    value: month,
  }));
  for (let i = 12; i < 16; i++) {
    monthsObjs[i] = {
      key: `${selectedDate.year() + 1}` + monthsObjs[i].key.substring(4),
      value: monthsObjs[i].value,
    };
  }
  return monthsObjs;
};
