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
  const prefix = period === "week" ? "on" : "and";

  const placeholder = useMemo(() => {
    const isEmpty = monthDays.length === 0;

    return isEmpty || isWeek ? "every day of the week" : "day of the week";
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
