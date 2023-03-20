import { DaysBody } from "./DaysBody";
import { DaysHeader } from "./DaysHeader";

export const Days = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  handleDateChange,
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
        handleDateChange={handleDateChange}
        selectedDate={selectedDate}
        minDate={minDate}
        maxDate={maxDate}
      />
    </>
  );
};
