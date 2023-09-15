import type { Dispatch, SetStateAction } from "react";
import type { FieldProps } from "../../types";

interface HoursProps extends FieldProps {
  hours: number[];
  setHours: Dispatch<SetStateAction<number[]>>;
}

export default HoursProps;
