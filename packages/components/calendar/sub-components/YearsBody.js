import moment from "moment";

import { YearsContainer } from "../styled-components";
import { getCalendarYears, getYearElements } from "../utils";

export const YearsBody = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  selectedDate
}) => {
  const years = getCalendarYears(observedDate);
  const yearElements = getYearElements(
    years,
    setObservedDate,
    setSelectedScene,
    selectedDate
  );

  return <YearsContainer>{yearElements}</YearsContainer>;
};
