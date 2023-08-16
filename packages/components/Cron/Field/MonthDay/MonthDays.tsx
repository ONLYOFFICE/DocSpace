import React, { useMemo } from "react";
import Select from "../../Select";
import { units } from "../../constants";
import MonthDaysProps from "./MonthDays.props";

function MonthDays({ weekDays, monthDays, setMonthDays }: MonthDaysProps) {
  const placeholder = useMemo(() => {
    const isEmpty = weekDays.length === 0;

    return isEmpty ? "every day of the month" : "day of the month";
  }, [weekDays.length]);

  return (
    <Select
      value={monthDays}
      setValue={setMonthDays}
      placeholder={placeholder}
      unit={units[2]}
      prefix="on"
      dropDownMaxHeight={300}
    />
  );
}
export default MonthDays;
