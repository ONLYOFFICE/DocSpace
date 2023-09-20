import React, { memo } from "react";
import Select from "../../Select";

import type MinutesProps from "./Minutes.props";

function Minutes({ minutes, setMinutes, period, t, unit }: MinutesProps) {
  const isHour = period === "Hour";
  const prefix = isHour ? t("At") : ":";

  return (
    <Select
      value={minutes}
      setValue={setMinutes}
      placeholder={t("EveryMinute")}
      unit={unit}
      prefix={prefix}
      dropDownMaxHeight={300}
    />
  );
}

export default memo(Minutes);
