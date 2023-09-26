import React, { memo } from "react";

import Select from "../../Select";

import type MonthsProps from "./Months.props";

function Months({ months, unit, setMonths, t }: MonthsProps) {
  return (
    <Select
      value={months}
      setValue={setMonths}
      placeholder={t("EveryMonth")}
      unit={unit}
      prefix={t("In")}
      dropDownMaxHeight={300}
    />
  );
}

export default memo(Months);
