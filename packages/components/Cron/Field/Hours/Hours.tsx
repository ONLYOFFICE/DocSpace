import React from "react";
import Select from "../../Select";
import { units } from "../../constants";
import HoursProps from "./Hours.props";

function Hours({ hours, setHours }: HoursProps) {
  return (
    <Select
      value={hours}
      setValue={setHours}
      placeholder="every hour"
      unit={units[1]}
      prefix="at"
      dropDownMaxHeight={300}
    />
  );
}

export default Hours;
