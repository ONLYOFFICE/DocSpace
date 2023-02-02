import { CurrentDateItem, DateItem, SecondaryDateItem } from "../styled-components";
import { MonthsContainer } from "../styled-components/MonthsContainer";
import moment from "moment";

export const MonthsBody = ({selectedDate}) => {
  const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar', 'Apr'];

  console.log(`${selectedDate.year()}-${moment().month('Jan').format('M')}`)

  const monthsElements = months.map(month => <DateItem big>{month}</DateItem>);
  for(let i = 12; i < 16; i++) {
    monthsElements[i] = <SecondaryDateItem big>{months[i]}</SecondaryDateItem>;
  }

  monthsElements[0] = <CurrentDateItem big>{months[0]}</CurrentDateItem>;

  return <MonthsContainer>{monthsElements}</MonthsContainer>;
}