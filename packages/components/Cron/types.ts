export type PeriodType = "Year" | "Month" | "Week" | "Day" | "Hour" | "Minute";

export type Unit = {
  name: "minute" | "hour" | "day" | "month" | "weekday";
  min: number;
  max: number;
  alt?: ReadonlyArray<string>;
  fullLabel?: ReadonlyArray<string>;
  total: number;
};

export type Options = {
  outputHashes: boolean;
  outputWeekdayNames: boolean;
  outputMonthNames: boolean;
};

export type Option<K = unknown, L = unknown> = {
  key: K;
  label: L;
};
