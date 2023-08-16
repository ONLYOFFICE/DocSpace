import React from "react";

import Select from "../../Select";
import { units } from "../../constants";

import MonthsProps from "./Months.props";

function Months({ months, setMonths }: MonthsProps) {
  return (
    <Select
      value={months}
      setValue={setMonths}
      placeholder="every month"
      unit={units[3]}
      prefix="in"
      dropDownMaxHeight={300}
    />
  );
}

export default Months;
