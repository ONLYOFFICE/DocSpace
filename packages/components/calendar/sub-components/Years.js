import { YearsBody } from "./YearsBody";
import { YearsHeader } from "./YearsHeader";

export const Years = ({ observedDate, setObservedDate, setSelectedScene, selectedDate }) => {
  return (
    <>
      <YearsHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
      />
      <YearsBody
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        selectedDate={selectedDate}
      />
    </>
  );
};
