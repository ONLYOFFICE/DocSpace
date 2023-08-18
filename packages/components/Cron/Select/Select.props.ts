import { Dispatch, SetStateAction } from "react";
import { Unit } from "./../types";
interface SelectProps {
  unit: Unit;
  value: number[];
  placeholder: string;
  setValue: Dispatch<SetStateAction<number[]>>;
  prefix: string;
  dropDownMaxHeight?: number;
  withTranslationPrefix?: boolean;
}

export default SelectProps;
