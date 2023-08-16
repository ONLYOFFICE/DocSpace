import type { Options, PeriodType, Unit } from "./types";

export const defaultPeriod: PeriodType = "hour";

export const numberOfMonth: Record<string, number> = {
  JAN: 1,
  FEB: 2,
  MAR: 3,
  APR: 4,
  MAY: 5,
  JUN: 6,
  JUL: 7,
  AUG: 8,
  SEP: 9,
  OCT: 10,
  NOV: 11,
  DEC: 12,
};

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
