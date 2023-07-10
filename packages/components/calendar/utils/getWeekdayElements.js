import React from "react";
import moment from "moment";

import { Weekday } from "../styled-components";

export const getWeekdayElements = () => {
  const weekdays = moment
    .weekdaysMin(true)
    .map((weekday) => weekday.charAt(0).toUpperCase() + weekday.substring(1));
  return weekdays.map((day) => (
    <Weekday className="weekday" key={day}>
      {day}
    </Weekday>
  ));
};
