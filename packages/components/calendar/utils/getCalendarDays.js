import moment from "moment";

export const getCalendarDays = (date) => {
  const selectedDate = moment(date);

  const prevMonthDays = [];
  const currentMonthDays = [];
  const nextMonthDays = [];
  const maxCalendarDays = 42;

  const monthFirstMonday = selectedDate.clone().startOf("month").weekday(1).date();
  const firstCalendarMonday =
    monthFirstMonday < 10
      ? selectedDate.clone().startOf("month").subtract(1, "week").weekday(1).date()
      : monthFirstMonday;

  for (let i = 1; i <= selectedDate.clone().daysInMonth(); i++) {
    currentMonthDays.push({
      key: selectedDate.clone().format("YYYY-MM") + "-" + i,
      value: i,
    });
  }

  if (monthFirstMonday !== 1) {
    const prevMonthLength = selectedDate.clone().subtract(1, "months").daysInMonth();
    for (let i = firstCalendarMonday; i <= prevMonthLength; i++) {
      prevMonthDays.push({
        key: selectedDate.clone().subtract(1, "months").format("YYYY-MM") + "-" + i,
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
      key: selectedDate.clone().add(1, "months").format("YYYY-MM") + "-" + i,
      value: i,
    });
  }

  return { prevMonthDays, currentMonthDays, nextMonthDays };
};
