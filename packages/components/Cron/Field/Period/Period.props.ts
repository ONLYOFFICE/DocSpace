import type { Dispatch, SetStateAction } from "react";
import type { PeriodType, TFunction } from "../../types";

interface PeriodProps {
  t: TFunction;
  period?: PeriodType;
  setPeriod: Dispatch<SetStateAction<PeriodType>>;
}

export type PeriodOptionType = {
  key: PeriodType;
  label: string;
};

export default PeriodProps;
