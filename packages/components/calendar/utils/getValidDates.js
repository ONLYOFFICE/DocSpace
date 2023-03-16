import moment from "moment";

export const getValidDates = (
  currentMinDate,
  currentMaxDate,
  minDate = new Date("01/01/1970"),
  maxDate = new Date().setFullYear(new Date().getFullYear() + 10)
) => {
  if (minDate >= maxDate) {
    minDate = new Date("01/01/1970");
    maxDate = new Date().setFullYear(new Date().getFullYear() + 10);
    console.error(
      "The minimum date is farther than or same as the maximum date. minDate and maxDate are set to default"
    );
  }
  minDate = moment(minDate);
  maxDate = moment(maxDate);

  let resultMinDate = moment(currentMinDate);
  let resultMaxDate = moment(currentMaxDate);

  resultMinDate = resultMinDate < minDate ? minDate : resultMinDate;
  resultMaxDate = resultMaxDate > maxDate ? maxDate : resultMaxDate;

  if (resultMinDate >= resultMaxDate) {
    resultMinDate = minDate;
    resultMaxDate = maxDate;
  }

  return [resultMinDate, resultMaxDate];
};
