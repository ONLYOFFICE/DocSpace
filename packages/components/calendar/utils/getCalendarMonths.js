import moment from "moment";

export const getCalendarMonths = (observedDate) => {
  // prettier-ignore
  const months = moment.monthsShort();

  const monthsObjs = months.map((month) => ({
    key: `${observedDate.year()}-${moment().month(month).format("M")}`,
    value: month,
  }));
  for (let i = 0; i < 4; i++) {
    monthsObjs.push({
      key: `${observedDate.year() + 1}-${moment()
        .month(months[i])
        .format("M")}`,
      value: monthsObjs[i].value,
    });
  }
  return monthsObjs;
};
