import { Dispatch, SetStateAction } from "react";

interface HoursProps {
  hours: number[];
  setHours: Dispatch<SetStateAction<number[]>>;
}

export default HoursProps;
