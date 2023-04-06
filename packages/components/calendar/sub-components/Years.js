import { YearsBody } from "./YearsBody";
import { YearsHeader } from "./YearsHeader";

export const Years = ({ observedDate, setObservedDate, setSelectedScene, selectedDate, minDate, maxDate }) => {
  return (
    <>
      <YearsHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        minDate={minDate}
        maxDate={maxDate}
      />
      <YearsBody
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        selectedDate={selectedDate}
        minDate={minDate}
        maxDate={maxDate}
      />
    </>
  );
};
