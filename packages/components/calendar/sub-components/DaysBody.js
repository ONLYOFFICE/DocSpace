import { DateItem, DaysContainer, SecondaryDateItem, Weekday } from "../styled-components";

export const DaysBody = () => {
  const weekdays = ["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"];
  const weekdaysElements = weekdays.map((day) => <Weekday>{day}</Weekday>);

  const days = [26, 27, 28, 29, 30, 31, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 1, 2, 3, 4, 5];

  const firstDay = days.indexOf(1);
  const lastDay = days.indexOf(1, firstDay+1);

  const daysElements = days.map(day => <SecondaryDateItem>{day}</SecondaryDateItem>);

  for(let i = firstDay; i < lastDay; i++){
    daysElements[i] = <DateItem>{days[i]}</DateItem>; 
  }

  console.log('firstDay ', firstDay, ' lastDay ', lastDay);

  return <DaysContainer>{weekdaysElements} {daysElements}</DaysContainer>;
};
