import type { Dispatch, SetStateAction } from "react";
import type { FieldProps } from "../../types";

interface MonthsProps extends FieldProps {
  months: number[];
  setMonths: Dispatch<SetStateAction<number[]>>;
}

export default MonthsProps;
