import { Options, PeriodType } from "./types";
import { defaultOptions, units } from "./constants";
import { arrayToStringPart, assertValidArray, stringToArrayPart } from "./util";

export const stringToArray = (str: string) => {
  if (typeof str !== "string") {
    throw new Error("Invalid cron string");
  }
  const parts = str.replace(/\s+/g, " ").trim().split(" ");
  if (parts.length !== 5) {
    throw new Error("Invalid cron string format");
  } else {
    return parts.map((str, idx) => stringToArrayPart(str, units[idx]));
  }
};

export function arrayToString(arr: number[][], options?: Partial<Options>) {
  assertValidArray(arr);
  const parts = arr.map((part, idx) =>
    arrayToStringPart(part, units[idx], { ...defaultOptions, ...options })
  );
  return parts.join(" ");
}

export function getCronStringFromValues(
  period: PeriodType,
  months: number[] | undefined,
  monthDays: number[] | undefined,
  weekDays: number[] | undefined,
  hours: number[] | undefined,
  minutes: number[] | undefined
) {
  const newMonths = period === "Year" && months ? months : [];
  const newMonthDays =
    (period === "Year" || period === "Month") && monthDays ? monthDays : [];
  const newWeekDays =
    (period === "Year" || period === "Month" || period === "Week") && weekDays
      ? weekDays
      : [];
  const newHours =
    period !== "Minute" && period !== "Hour" && hours ? hours : [];
  const newMinutes = period !== "Minute" && minutes ? minutes : [];

  const parsedArray = arrayToString([
    newMinutes,
    newHours,
    newMonthDays,
    newMonths,
    newWeekDays,
  ]);

  return parsedArray;
}
