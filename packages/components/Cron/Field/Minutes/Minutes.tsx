import React from "react";
import Select from "../../Select";
import { units } from "../../constants";
import MinutesProps from "./Minutes.props";

function Minutes({ minutes, setMinutes, period }: MinutesProps) {
  const isHour = period === "Hour";
  const prefix = isHour ? "At" : ":";

  return (
    <Select
      value={minutes}
      setValue={setMinutes}
      placeholder="EveryMinute"
      unit={units[0]}
      prefix={prefix}
      dropDownMaxHeight={300}
      withTranslationPrefix={isHour}
    />
  );
}

export default Minutes;
