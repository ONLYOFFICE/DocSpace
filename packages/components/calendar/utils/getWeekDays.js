// weekdays = ['Su', 'Mo', 'Tu' ...etc]
// props = calendar component's props
// return [{key: 'en', value: 'Mo', disabled: false} ...]

export const getWeekDays = (weekdays, props) => {
  let arrayWeekDays = [];
  weekdays.push(weekdays.shift());
  for (let i = 0; i < weekdays.length; i++) {
    arrayWeekDays.push({
      key: `${props.locale}_${i}`,
      value: weekdays[i],
      disabled: i >= 5 ? true : false,
    });
  }
  return arrayWeekDays;
};
