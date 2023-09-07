import { DateTime } from "luxon";
import { Options, PeriodType } from "./types";
import { defaultOptions } from "./constants";
import {
  arrayToStringPart,
  assertValidArray,
  findDate,
  stringToArrayPart,
  getUnits,
} from "./util";

const units = getUnits();

export const stringToArray = (str: string, full = false) => {
  if (typeof str !== "string") {
    throw new Error("Invalid cron string");
  }
  const parts = str.replace(/\s+/g, " ").trim().split(" ");
  if (parts.length !== 5) {
    throw new Error("Invalid cron string format");
  } else {
    return parts.map((str, idx) => stringToArrayPart(str, units[idx], full));
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

export const getNextSynchronization = (
  cronString: string,
  timezone?: string
) => {
  try {
    const cron = stringToArray(cronString, true);
    assertValidArray(cron);
    let date = DateTime.now();

    if (timezone) date = date.setZone(timezone);

    if (!date.isValid) {
      throw new Error("Invalid timezone provided");
    }

    if (date.second > 0) {
      // plus a minute to the date to prevent returning dates in the past
      date = date.plus({ minute: 1 });
    }

    return findDate(cron, date);
  } catch (error) {
    console.log(error);
  }
};
