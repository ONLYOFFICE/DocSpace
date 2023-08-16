import type { Options, PeriodType, Unit } from "./types";

export const defaultPeriod: PeriodType = "Hour";

export const defaultOptions: Options = {
  outputHashes: false,
  outputMonthNames: false,
  outputWeekdayNames: false,
};

export const units: ReadonlyArray<Unit> = Object.freeze([
  {
    name: "minute",
    min: 0,
    max: 59,
    total: 60,
  },
  {
    name: "hour",
    min: 0,
    max: 23,
    total: 24,
  },
  {
    name: "day",
    min: 1,
    max: 31,
    total: 31,
  },
  {
    name: "month",
    min: 1,
    max: 12,
    total: 12,
    alt: [
      "JAN",
      "FEB",
      "MAR",
      "APR",
      "MAY",
      "JUN",
      "JUL",
      "AUG",
      "SEP",
      "OCT",
      "NOV",
      "DEC",
    ],
    fullLabel: [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ],
  },
  {
    name: "weekday",
    min: 0,
    max: 6,
    total: 7,
    alt: ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"],
  },
]);

export const defaultCronString = "* * * * *";
