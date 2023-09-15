import type { Dispatch, SetStateAction } from "react";
import type { FieldProps } from "../../types";

interface MonthDaysProps extends FieldProps {
  monthDays: number[];
  weekDays: number[];
  setMonthDays: Dispatch<SetStateAction<number[]>>;
}

export default MonthDaysProps;
