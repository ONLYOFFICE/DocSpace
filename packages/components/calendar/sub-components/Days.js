import { DaysBody } from "./DaysBody";
import { DaysHeader } from "./DaysHeader";

export const Days = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  setSelectedDate,
  selectedDate,
}) => {
  return (
    <>
      <DaysHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
      />
      <DaysBody
        observedDate={observedDate}
        setSelectedDate={setSelectedDate}
        selectedDate={selectedDate}
      />
    </>
  );
};
