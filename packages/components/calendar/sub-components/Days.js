import { DaysBody } from "./DaysBody";
import { DaysHeader } from "./DaysHeader";

export const Days = ({ selectedDate, setSelectedDate }) => {
  return (
    <>
      <DaysHeader
        selectedDate={selectedDate}
        setSelectedDate={setSelectedDate}
      />
      <DaysBody selectedDate={selectedDate} setSelectedDate={setSelectedDate} />
    </>
  );
};
