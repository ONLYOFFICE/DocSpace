import React from "react";
import Select from "../../Select";
import { units } from "../../constants";
import MinutesProps from "./Minutes.props";

function Minutes({ minutes, setMinutes, period }: MinutesProps) {
  const prefix = period === "hour" ? "at" : ":";

  return (
    <Select
      value={minutes}
      setValue={setMinutes}
      placeholder="every minute"
      unit={units[0]}
      prefix={prefix}
      dropDownMaxHeight={300}
    />
  );
}

export default Minutes;
