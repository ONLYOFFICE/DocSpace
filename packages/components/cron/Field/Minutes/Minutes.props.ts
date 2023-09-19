import type { Dispatch, SetStateAction } from "react";
import type { PeriodType, FieldProps } from "../../types";

interface MinutesProps extends FieldProps {
  minutes: number[];
  setMinutes: Dispatch<SetStateAction<number[]>>;
  period: PeriodType;
}

export default MinutesProps;
