import moment from "moment";
import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

const onDateClick = (dateString, setSelectedDate) =>
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().set({
      year: dateString.substring(0, 4),
      month: dateString.substring(5),
    })
  );

export const getMonthElements = (months, setSelectedDate) => {
  const monthsElements = months.map((month) => (
    <DateItem
      big
      key={month.key}
      onClick={() => onDateClick(month.key, setSelectedDate)}
    >
      {month.value}
    </DateItem>
  ));
  for (let i = 12; i < 16; i++) {
    monthsElements[i] = (
      <SecondaryDateItem
        big
        key={months[i].key}
        onClick={() => onDateClick(months[i].key, setSelectedDate)}
      >
        {months[i].value}
      </SecondaryDateItem>
    );
  }

  const currentDate = `${moment().year()}-${moment().format("M")}`;

  months.forEach((month, index) => {
    if (month.key === currentDate) {
      monthsElements[index] = (
        <CurrentDateItem
          big
          key={month.key}
          onClick={() => onDateClick(month.key, setSelectedDate)}
        >
          {month.value}
        </CurrentDateItem>
      );
    }
  });
  return monthsElements;
};
