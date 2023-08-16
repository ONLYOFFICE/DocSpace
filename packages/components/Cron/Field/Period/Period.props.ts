import { Dispatch, SetStateAction } from "react";
import { PeriodType } from "../../types";

interface PeriodProps {
  period?: PeriodType;
  setPeriod: Dispatch<SetStateAction<PeriodType>>;
}

export type PeriodOptionType = {
  key: PeriodType;
  label: string;
};

export default PeriodProps;
