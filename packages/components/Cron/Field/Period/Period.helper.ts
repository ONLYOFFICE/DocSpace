import { PeriodOptionType } from "./Period.props";
import { TFunction } from "react-i18next";

export const getOptions = (
  t: TFunction<"translation", undefined>
): PeriodOptionType[] => [
  {
    key: "year",
    label: t("Every year"),
  },
  {
    key: "month",
    label: t("Every month"),
  },
  {
    key: "week",
    label: t("Every week"),
  },
  {
    key: "day",
    label: t("Every day"),
  },
  {
    key: "hour",
    label: t("Every hour"),
  },
];
