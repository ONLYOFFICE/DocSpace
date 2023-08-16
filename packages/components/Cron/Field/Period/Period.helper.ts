import { PeriodOptionType } from "./Period.props";
import { TFunction } from "react-i18next";

export const getOptions = (
  t: TFunction<"translation", undefined>
): PeriodOptionType[] => [
  {
    key: "Year",
    label: t("EveryYear"),
  },
  {
    key: "Month",
    label: t("EveryMonth"),
  },
  {
    key: "Week",
    label: t("EveryWeek"),
  },
  {
    key: "Day",
    label: t("EveryDay"),
  },
  {
    key: "Hour",
    label: t("EveryHour"),
  },
];
