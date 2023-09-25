import React, { memo } from "react";
import Select from "../../Select";

import type HoursProps from "./Hours.props";

function Hours({ hours, setHours, unit, t }: HoursProps) {
  return (
    <Select
      value={hours}
      setValue={setHours}
      placeholder={t("EveryHour")}
      unit={unit}
      prefix={t("At")}
      dropDownMaxHeight={300}
    />
  );
}

export default memo(Hours);
