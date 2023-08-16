import { Dispatch, SetStateAction } from "react";
import { PeriodType } from "../../types";

interface WeekDaysProps {
  isWeek: boolean;
  period: PeriodType;
  weekDays: number[];
  monthDays: number[];
  setWeekDays: Dispatch<SetStateAction<number[]>>;
}

export default WeekDaysProps;
