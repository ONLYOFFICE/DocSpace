import type { Dispatch, SetStateAction } from "react";
import type { PeriodType, FieldProps } from "../../types";

interface WeekDaysProps extends FieldProps {
  isWeek: boolean;
  period: PeriodType;
  weekDays: number[];
  monthDays: number[];
  setWeekDays: Dispatch<SetStateAction<number[]>>;
}

export default WeekDaysProps;
