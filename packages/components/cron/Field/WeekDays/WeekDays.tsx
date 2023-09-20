import React, { useMemo, memo } from "react";

import Select from "../../Select";

import type WeekDaysProps from "./WeekDays.props";

function WeekDays({
  setWeekDays,
  unit,
  isWeek,
  weekDays,
  monthDays,
  period,
  t,
}: WeekDaysProps) {
  const prefix = period === "Week" ? t("On") : t("And");

  const placeholder = useMemo(() => {
    const isEmpty = monthDays.length === 0;

    return isEmpty || isWeek ? t("EveryDayOfTheWeek") : t("DayOfTheWeek");
  }, [monthDays.length, isWeek]);

  return (
    <Select
      value={weekDays}
      setValue={setWeekDays}
      placeholder={placeholder}
      unit={unit}
      prefix={prefix}
      dropDownMaxHeight={300}
    />
  );
}

export default memo(WeekDays);
