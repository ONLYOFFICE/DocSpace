import moment from "moment";

import { LANGUAGE } from "@docspace/common/constants";
import { getCookie } from "@docspace/common/utils";

export const getRelativeDateDay = (t, date) => {
  moment.locale(getCookie(LANGUAGE));

  const given = moment(date);

  const now = moment();
  const weekAgo = moment().subtract(1, "week");
  const halfYearAgo = moment().subtract(6, "month");

  if (given.isAfter(weekAgo)) {
    if (now.weekday() === given.weekday()) return t("InfoPanel:Today");
    if (now.weekday() - 1 === given.weekday()) return t("InfoPanel:Yesterday");

    const weekday = moment.weekdays(given.weekday());
    return weekday.charAt(0).toUpperCase() + weekday.slice(1);
  }

  if (given.isBetween(halfYearAgo, weekAgo)) {
    const shortDate = given.format("MMMM D");
    return shortDate.charAt(0).toUpperCase() + shortDate.slice(1);
  }

  const longDate = given.format("MMMM D, YYYY");
  return longDate.charAt(0).toUpperCase() + longDate.slice(1);
};

export const getDateTime = (date) => {
  moment.locale(getCookie(LANGUAGE));

  const given = moment(date);
  return given.format("LT");
};
