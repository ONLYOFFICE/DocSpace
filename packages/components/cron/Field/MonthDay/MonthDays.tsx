import React, { useMemo, memo } from "react";
import Select from "../../Select";

import type MonthDaysProps from "./MonthDays.props";

function MonthDays({
  weekDays,
  monthDays,
  unit,
  setMonthDays,
  t,
}: MonthDaysProps) {
  const placeholder = useMemo(() => {
    const isEmpty = weekDays.length === 0;

    return isEmpty ? t("EveryDayOfTheMonth") : t("DayOfTheMonth");
  }, [weekDays.length]);

  return (
    <Select
      value={monthDays}
      setValue={setMonthDays}
      placeholder={placeholder}
      unit={unit}
      prefix={t("On")}
      dropDownMaxHeight={300}
    />
  );
}
export default memo(MonthDays);
