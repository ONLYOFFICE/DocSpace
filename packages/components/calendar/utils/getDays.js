import { getFirstMonthDay } from "./getFirstMonthDay";
import { compareMonths } from "./compareMonths";

// returns [{className: "calendar-month_neighboringMonth", dayState: "prev", disableClass: null, value: 26} ...]

export const getDays = (minDate, maxDate, openToDate, selectedDate) => {
  const currentYear = openToDate.getFullYear();
  const currentMonth = openToDate.getMonth() + 1;
  const countDaysInMonth = new Date(currentYear, currentMonth, 0).getDate();
  let countDaysInPrevMonth = new Date(
    currentYear,
    currentMonth - 1,
    0
  ).getDate();
  const arrayDays = [];
  let className = "calendar-month_neighboringMonth";

  const openToDateMonth = openToDate.getMonth();
  const openToDateYear = openToDate.getFullYear();

  const maxDateMonth = maxDate.getMonth();
  const maxDateYear = maxDate.getFullYear();
  const maxDateDay = maxDate.getDate();

  const minDateMonth = minDate.getMonth();
  const minDateYear = minDate.getFullYear();
  const minDateDay = minDate.getDate();

  //Disable preview month
  let disableClass = null;
  if (openToDateYear === minDateYear && openToDateMonth === minDateMonth) {
    disableClass = "calendar-month_disabled";
  }

  //Prev month
  let prevMonthDay = null;
  if (openToDateYear === minDateYear && openToDateMonth - 1 === minDateMonth) {
    prevMonthDay = minDateDay;
  }

  //prev month + year
  let prevYearDay = null;
  if (
    openToDateYear === minDateYear + 1 &&
    openToDateMonth === 0 &&
    minDateMonth === 11
  ) {
    prevYearDay = minDateDay;
  }

  // Show neighboring days in prev month
  const firstDayOfMonth = getFirstMonthDay(openToDate);

  for (let i = firstDayOfMonth; i != 0; i--) {
    if (countDaysInPrevMonth + 1 === prevMonthDay) {
      disableClass = "calendar-month_disabled";
    }
    if (countDaysInPrevMonth + 1 === prevYearDay) {
      disableClass = "calendar-month_disabled";
    }
    arrayDays.unshift({
      value: countDaysInPrevMonth--,
      disableClass,
      className,
      dayState: "prev",
    });
  }

  //Disable max days in month
  let maxDay, minDay;
  disableClass = null;
  if (openToDateYear === maxDateYear && openToDateMonth >= maxDateMonth) {
    if (openToDateMonth === maxDateMonth) {
      maxDay = maxDateDay;
    } else {
      maxDay = null;
    }
  }

  //Disable min days in month
  if (openToDateYear === minDateYear && openToDateMonth >= minDateMonth) {
    if (openToDateMonth === minDateMonth) {
      minDay = minDateDay;
    } else {
      minDay = null;
    }
  }

  // Show days in month and weekend days
  let seven = 7;
  const dateNow = selectedDate.getDate();

  for (let i = 1; i <= countDaysInMonth; i++) {
    if (i === seven - firstDayOfMonth - 1) {
      className = "calendar-month_weekend";
    } else if (i === seven - firstDayOfMonth) {
      seven += 7;
      className = "calendar-month_weekend";
    } else {
      className = "calendar-month";
    }
    if (i === dateNow && compareMonths(openToDate, selectedDate) === 0) {
      className = "calendar-month_selected-day";
    }
    if (i > maxDay || i < minDay) {
      disableClass = "calendar-month_disabled";
      className = "calendar-month_disabled";
    } else {
      disableClass = null;
    }

    arrayDays.push({
      value: i,
      disableClass,
      className,
      dayState: "now",
    });
  }

  //Calculating neighboring days in next month
  let maxDaysInMonthTable = 42;
  const maxDaysInMonth = 42;
  if (firstDayOfMonth > 5 && countDaysInMonth >= 30) {
    maxDaysInMonthTable += 7;
  } else if (firstDayOfMonth >= 5 && countDaysInMonth > 30) {
    maxDaysInMonthTable += 7;
  }
  if (maxDaysInMonthTable > maxDaysInMonth) {
    maxDaysInMonthTable -= 7;
  }

  //Disable next month days
  disableClass = null;
  if (openToDateYear === maxDateYear && openToDateMonth >= maxDateMonth) {
    disableClass = "calendar-month_disabled";
  }

  //next month + year
  let nextYearDay = null;
  if (
    openToDateYear === maxDateYear - 1 &&
    openToDateMonth === 11 &&
    maxDateMonth === 0
  ) {
    nextYearDay = maxDateDay;
  }

  //next month
  let nextMonthDay = null;
  if (openToDateYear === maxDateYear && openToDateMonth === maxDateMonth - 1) {
    nextMonthDay = maxDateDay;
  }

  //Show neighboring days in next month
  let dayInNextMonth = 1;
  className = "calendar-month_neighboringMonth";
  for (
    let i = countDaysInMonth;
    i < maxDaysInMonthTable - firstDayOfMonth;
    i++
  ) {
    if (i - countDaysInMonth === nextYearDay) {
      disableClass = "calendar-month_disabled";
    }
    if (i - countDaysInMonth === nextMonthDay) {
      disableClass = "calendar-month_disabled";
    }
    arrayDays.push({
      value: dayInNextMonth++,
      disableClass,
      className,
      dayState: "next",
    });
  }
  return arrayDays;
};
