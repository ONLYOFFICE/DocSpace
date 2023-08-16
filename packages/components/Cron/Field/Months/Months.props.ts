import { Dispatch, SetStateAction } from "react";

interface MonthsProps {
  months: number[];
  setMonths: Dispatch<SetStateAction<number[]>>;
}

export default MonthsProps;
