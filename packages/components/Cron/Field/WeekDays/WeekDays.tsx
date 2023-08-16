import React, { useMemo } from "react";
import Select from "../../Select";
import { units } from "../../constants";
import WeekDaysProps from "./WeekDays.props";

function WeekDays({
  setWeekDays,
  isWeek,
  weekDays,
  monthDays,
  period,
}: WeekDaysProps) {
  const prefix = period === "Week" ? "On" : "And";

  const placeholder = useMemo(() => {
    const isEmpty = monthDays.length === 0;

    return isEmpty || isWeek ? "EveryDayOfTheWeek" : "DayOfTheWeek";
  }, [monthDays.length, isWeek]);

  return (
    <Select
      value={weekDays}
      setValue={setWeekDays}
      placeholder={placeholder}
      unit={units[4]}
      prefix={prefix}
      dropDownMaxHeight={300}
    />
  );
}

export default WeekDays;
