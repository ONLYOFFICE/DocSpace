import moment from "moment";
import {
  CurrentDateItem,
  DateItem,
  SecondaryDateItem,
} from "../styled-components";

const onDateClick = (dateString, setObservedDate, setSelectedScene) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().set({
      year: dateString.substring(0, 4),
      month: dateString.substring(5) - 1,
    })
  );
  setSelectedScene((prevSelectedScene) => prevSelectedScene - 1);
};

export const getMonthElements = (
  months,
  setObservedDate,
  setSelectedScene,
  selectedDate,
  minDate,
  maxDate
) => {
  const onClick = (dateString) =>
    onDateClick(dateString, setObservedDate, setSelectedScene);

  const monthsElements = months.map((month) => (
    <DateItem
      big
      key={month.key}
      onClick={() => onClick(month.key)}
      disabled={
        moment(month.key).endOf("month") < minDate ||
        moment(month.key).startOf("month") > maxDate
      }
    >
      {month.value}
    </DateItem>
  ));
  for (let i = 12; i < 16; i++) {
    monthsElements[i] = (
      <SecondaryDateItem
        big
        key={months[i].key}
        onClick={() => onClick(months[i].key)}
        disabled={
          moment(months[i].key).endOf("month") < minDate ||
          moment(months[i].key).startOf("month") > maxDate
        }
      >
        {months[i].value}
      </SecondaryDateItem>
    );
  }

  const currentDate = `${moment().year()}-${moment().format("M")}`;
  const formattedDate = `${moment(selectedDate).year()}-${moment(
    selectedDate
  ).format("M")}`;

  months.forEach((month, index) => {
    if (month.key === currentDate) {
      monthsElements[index] = (
        <CurrentDateItem
          big
          key={month.key}
          onClick={() => onClick(month.key)}
          disabled={
            moment(month.key).endOf("month") < minDate ||
            moment(month.key).startOf("month") > maxDate
          }
        >
          {month.value}
        </CurrentDateItem>
      );
    } else if (month.key === formattedDate) {
      monthsElements[index] = (
        <DateItem
          big
          key={month.key}
          focused
          onClick={() => onClick(month.key)}
          disabled={
            moment(month.key).endOf("month") < minDate ||
            moment(month.key).startOf("month") > maxDate
          }
        >
          {month.value}
        </DateItem>
      );
    }
  });
  return monthsElements;
};
