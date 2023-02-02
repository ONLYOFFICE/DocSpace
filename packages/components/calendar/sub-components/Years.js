import { YearsBody } from "./YearsBody";
import { YearsHeader } from "./YearsHeader";

export const Years = ({ selectedDate, setSelectedDate }) => {
  return (
    <>
      <YearsHeader selectedDate={selectedDate} setSelectedDate={setSelectedDate} />
      <YearsBody selectedDate={selectedDate} setSelectedDate={setSelectedDate} />
    </>
  );
};
