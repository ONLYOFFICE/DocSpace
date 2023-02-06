import { DaysBody } from "./DaysBody";
import { DaysHeader } from "./DaysHeader";

export const Days = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  setSelectedDate,
  selectedDate,
  minDate,
  maxDate,
}) => {
  return (
    <>
      <DaysHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        minDate={minDate}
        maxDate={maxDate}
      />
      <DaysBody
        observedDate={observedDate}
        setSelectedDate={setSelectedDate}
        selectedDate={selectedDate}
        minDate={minDate}
        maxDate={maxDate}
      />
    </>
  );
};
