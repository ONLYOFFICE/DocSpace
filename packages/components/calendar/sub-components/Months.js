import React from "react";
import { MonthsBody } from "./MonthsBody";
import { MonthsHeader } from "./MonthsHeader";

export const Months = ({
  observedDate,
  setObservedDate,
  selectedDate,
  setSelectedScene,
  minDate,
  maxDate,
  isMobile
}) => {
  return (
    <>
      <MonthsHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        minDate={minDate}
        maxDate={maxDate}
      />
      <MonthsBody
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        selectedDate={selectedDate}
        minDate={minDate}
        maxDate={maxDate}
        isMobile={isMobile}
      />
    </>
  );
};
