import moment from "moment";

export default function getCorrectDate(locale, date) {
  //  if something went wrong with 'moment' - change for on this method get correct time
  //   const options = {
  //     year: "numeric",
  //     month: "2-digit",
  //     day: "2-digit",
  //     hour: "2-digit",
  //     minute: "numeric",
  //   };

  //   const correctDate = new Date(date)
  //     .toLocaleString(locale, options)
  //     .replace(",", "");

  const curDate = moment(date).locale(locale).format("L");
  const curTime = moment(date).locale(locale).format("LT");

  const correctDate = curDate + " " + curTime;

  return correctDate;
}
