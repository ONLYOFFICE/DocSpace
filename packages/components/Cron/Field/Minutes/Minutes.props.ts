import { Dispatch, SetStateAction } from "react";
import { PeriodType } from "../../types";

interface MinutesProps {
  minutes: number[];
  setMinutes: Dispatch<SetStateAction<number[]>>;
  period: PeriodType;
}

export default MinutesProps;
