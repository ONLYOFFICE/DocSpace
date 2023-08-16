import { Dispatch, SetStateAction } from "react";

interface MonthDaysProps {
  monthDays: number[];
  weekDays: number[];
  setMonthDays: Dispatch<SetStateAction<number[]>>;
}

export default MonthDaysProps;
