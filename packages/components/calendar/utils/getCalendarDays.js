import moment from "moment";

export const getCalendarDays = (date) => {
  const observedDate = moment(date);

  const prevMonthDays = [];
  const currentMonthDays = [];
  const nextMonthDays = [];
  const maxCalendarDays = 42;

  const firstCalendarMonday = observedDate
    .clone()
    .startOf("month")
    .startOf("week")
    .date();

  let yearMonthDate = observedDate.clone().format("YYYY-MM-");

  for (let i = 1; i <= observedDate.clone().daysInMonth(); i++) {
    currentMonthDays.push({
      key: yearMonthDate + moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
      value: moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
    });
  }

  if (firstCalendarMonday !== 1) {
    const prevMonthLength = observedDate
      .clone()
      .subtract(1, "months")
      .daysInMonth();

    yearMonthDate = observedDate
      .clone()
      .subtract(1, "months")
      .format("YYYY-MM-");

    for (let i = firstCalendarMonday; i <= prevMonthLength; i++) {
      prevMonthDays.push({
        key: yearMonthDate + moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
        value: moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
      });
    }
  }

  yearMonthDate = observedDate.clone().add(1, "months").format("YYYY-MM-");

  for (
    let i = 1;
    i <= maxCalendarDays - currentMonthDays.length - prevMonthDays.length;
    i++
  ) {
    nextMonthDays.push({
      key: yearMonthDate + moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
      value: moment(yearMonthDate + i, "YYYY-MM-D").format("D"),
    });
  }

  return { prevMonthDays, currentMonthDays, nextMonthDays };
};
