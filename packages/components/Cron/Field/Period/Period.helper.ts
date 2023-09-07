import type { PeriodOptionType } from "./Period.props";
import type { PeriodType, TFunction } from "../../types";

export const getOptions = (t: TFunction): PeriodOptionType[] => [
  {
    key: "Year",
    label: getLabel("Year", t),
  },
  {
    key: "Month",
    label: getLabel("Month", t),
  },
  {
    key: "Week",
    label: getLabel("Week", t),
  },
  {
    key: "Day",
    label: getLabel("Day", t),
  },
  {
    key: "Hour",
    label: getLabel("Hour", t),
  },
];

export const getLabel = (period: PeriodType, t: TFunction) => {
  switch (period) {
    case "Year":
      return t("EveryYear");
    case "Month":
      return t("EveryMonth");
    case "Week":
      return t("EveryWeek");
    case "Day":
      return t("EveryDay");
    case "Hour":
      return t("EveryHour");
    default:
      return "";
  }
};
