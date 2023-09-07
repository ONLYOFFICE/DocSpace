import type { Options, PeriodType, Unit } from "./types";

export const defaultPeriod: PeriodType = "Hour";

export const defaultOptions: Options = {
  outputHashes: false,
  outputMonthNames: false,
  outputWeekdayNames: false,
};

export const defaultCronString = "* * * * *";
