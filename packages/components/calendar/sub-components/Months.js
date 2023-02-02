import { MonthsBody } from "./MonthsBody";
import { MonthsHeader } from "./MonthsHeader";

export const Months = ({selectedDate, setSelectedDate}) => {
  return (
    <>
      <MonthsHeader
        selectedDate={selectedDate}
        setSelectedDate={setSelectedDate}
      />
      <MonthsBody
        selectedDate={selectedDate}
        setSelectedDate={setSelectedDate}
      />
    </>
  );
};
