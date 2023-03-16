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

  for (let i = 1; i <= observedDate.clone().daysInMonth(); i++) {
    currentMonthDays.push({
      key: observedDate.clone().format("YYYY-MM") + "-" + i,
      value: i,
    });
  }

  if (firstCalendarMonday !== 1) {
    const prevMonthLength = observedDate
      .clone()
      .subtract(1, "months")
      .daysInMonth();
    for (let i = firstCalendarMonday; i <= prevMonthLength; i++) {
      prevMonthDays.push({
        key:
          observedDate.clone().subtract(1, "months").format("YYYY-MM") +
          "-" +
          i,
        value: i,
      });
    }
  }

  for (
    let i = 1;
    i <= maxCalendarDays - currentMonthDays.length - prevMonthDays.length;
    i++
  ) {
    nextMonthDays.push({
      key: observedDate.clone().add(1, "months").format("YYYY-MM") + "-" + i,
      value: i,
    });
  }

  return { prevMonthDays, currentMonthDays, nextMonthDays };
};
