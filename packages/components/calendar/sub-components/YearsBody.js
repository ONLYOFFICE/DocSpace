import moment from "moment";

import {
  YearsContainer,
} from "../styled-components";
import { getCalendarYears, getYearElements } from "../utils";

export const YearsBody = ({ selectedDate, setSelectedDate }) => {
  const years = getCalendarYears(selectedDate);
  const yearElements = getYearElements(years, setSelectedDate);

  return <YearsContainer>{yearElements}</YearsContainer>;
};
