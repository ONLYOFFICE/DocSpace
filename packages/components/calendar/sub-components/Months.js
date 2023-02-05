import { MonthsBody } from "./MonthsBody";
import { MonthsHeader } from "./MonthsHeader";

export const Months = ({ observedDate, setObservedDate, selectedDate, setSelectedScene }) => {
  return (
    <>
      <MonthsHeader
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
      />
      <MonthsBody
        observedDate={observedDate}
        setObservedDate={setObservedDate}
        setSelectedScene={setSelectedScene}
        selectedDate={selectedDate}
      />
    </>
  );
};
