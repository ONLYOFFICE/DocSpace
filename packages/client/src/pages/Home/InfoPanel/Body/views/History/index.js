import React, { useState, useEffect } from "react";

import {
  StyledHistoryBlock,
  StyledHistoryList,
  StyledHistorySubtitle,
} from "../../styles/history";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import getCorrectDate from "@docspace/components/utils/getCorrectDate";
import { getUser } from "@docspace/common/api/people";
import { parseAndFormatDate } from "../../helpers/DetailsHelper";
import HistoryBlockMessage from "./HistoryBlockMessage";
import HistoryBlockItemList from "./HistoryBlockItemList";

const History = ({
  t,
  selection,
  setSelection,
  personal,
  culture,
  getItemIcon,

  getRoomHistory,
  openFileAction,
}) => {
  const [history, setHistory] = useState(null);

  const parseHistoryJSON = async (fetchedHistory) => {
    let feeds = fetchedHistory.feeds;
    let newFeeds = [];
    for (let i = 0; i < feeds.length; i++) {
      const feedsJSON = JSON.parse(feeds[i].json);
      feedsJSON.author = await getUser(feedsJSON.AuthorId);

      let groupFeeds = feeds[i].groupedFeeds;
      let newGroupFeeds = [];
      for (let j = 0; j < groupFeeds.length; j++) {
        const groupFeedsJSON = JSON.parse(groupFeeds[j].json);
        groupFeedsJSON.author = await getUser(groupFeedsJSON.AuthorId);
        newGroupFeeds.push(groupFeedsJSON);
      }

      newFeeds.push({
        ...feeds[i],
        json: feedsJSON,
        groupedFeeds: newGroupFeeds,
      });
    }

    return { ...fetchedHistory, feeds: newFeeds };
  };

  useEffect(async () => {
    if (selection.history) {
      setHistory(selection.history);
      return;
    }

    if (!selection.isRoom) return;
    let fetchedHistory = await getRoomHistory(selection.id);
    fetchedHistory = await parseHistoryJSON(fetchedHistory);
    console.log(fetchedHistory);

    setHistory(fetchedHistory);
    setSelection({ ...selection, history: fetchedHistory });
  }, [selection]);

  if (!selection || !history) return null;
  return (
    <>
      <StyledHistoryList>
        <StyledHistorySubtitle>{t("RecentActivities")}</StyledHistorySubtitle>

        {history.feeds.map((feed) => (
          <StyledHistoryBlock key={feed.json.ModifiedDate}>
            <Avatar
              role="user"
              className="avatar"
              size="min"
              source={
                feed.json.author.avatar ||
                (feed.json.author.displayName
                  ? ""
                  : feed.json.author.email && "/static/images/@.react.svg")
              }
              userName={feed.json.author.displayName}
            />
            <div className="info">
              <div className="title">
                <Text className="name">{feed.json.author.displayName}</Text>
                {feed.json.author.isOwner && (
                  <Text className="secondary-info">
                    {t("Common:Owner").toLowerCase()}
                  </Text>
                )}
                <Text className="date">
                  {parseAndFormatDate(
                    feed.json.ModifiedDate,
                    personal,
                    culture
                  )}
                </Text>
              </div>

              <HistoryBlockMessage
                t={t}
                action={feed.json}
                groupedActions={feed.groupedFeeds}
              />

              <HistoryBlockItemList
                items={[feed.json, ...feed.groupedFeeds]}
                getItemIcon={getItemIcon}
                openFileAction={openFileAction}
              />
              {/* <HistoryBlockContent t={t} feed={feed.json} /> */}
            </div>
          </StyledHistoryBlock>
        ))}
      </StyledHistoryList>
    </>
  );
};

export default History;
