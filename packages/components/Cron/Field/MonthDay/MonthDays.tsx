import React, { useMemo } from "react";
import Select from "../../Select";
import { units } from "../../constants";
import MonthDaysProps from "./MonthDays.props";

function MonthDays({ weekDays, monthDays, setMonthDays }: MonthDaysProps) {
  const placeholder = useMemo(() => {
    const isEmpty = weekDays.length === 0;

    return isEmpty ? "EveryDayOfTheMonth" : "DayOfTheMonth";
  }, [weekDays.length]);

  return (
    <Select
      value={monthDays}
      setValue={setMonthDays}
      placeholder={placeholder}
      unit={units[2]}
      prefix="On"
      dropDownMaxHeight={300}
    />
  );
}
export default MonthDays;
