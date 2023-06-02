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
    if (now.weekday() === given.weekday()) return t("Common:Today");
    if (now.weekday() - 1 === given.weekday()) return t("Common:Yesterday");

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

// from { response: { feeds: groupedFeeds: [{ json: "" }], json: "" }}
//   to [{ day: "", feeds: [ groupedFeeds: [{ json: {} }], json: {} ]}]

export const parseHistory = (t, fetchedHistory) => {
  let feeds = fetchedHistory.feeds;
  let parsedFeeds = [];

  for (let i = 0; i < feeds.length; i++) {
    const feedsJSON = JSON.parse(feeds[i].json);
    const feedDay = getRelativeDateDay(t, feeds[i].modifiedDate);

    let newGroupedFeeds = [];
    if (feeds[i].groupedFeeds) {
      let groupFeeds = feeds[i].groupedFeeds;
      for (let j = 0; j < groupFeeds.length; j++)
        newGroupedFeeds.push(
          !!groupFeeds[j].target
            ? groupFeeds[j].target
            : JSON.parse(groupFeeds[j].json)
        );
    }

    if (parsedFeeds.length && parsedFeeds.at(-1).day === feedDay)
      parsedFeeds.at(-1).feeds.push({
        ...feeds[i],
        json: feedsJSON,
        groupedFeeds: newGroupedFeeds,
      });
    else
      parsedFeeds.push({
        day: feedDay,
        feeds: [
          {
            ...feeds[i],
            json: feedsJSON,
            groupedFeeds: newGroupedFeeds,
          },
        ],
      });
  }

  return parsedFeeds;
};
