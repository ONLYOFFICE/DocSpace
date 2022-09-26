import React, { useState, useEffect } from "react";

import {
  StyledHistoryBlock,
  StyledHistoryList,
  StyledHistorySubtitle,
} from "../../styles/history";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import { getUser } from "@docspace/common/api/people";
import { parseAndFormatDate } from "../../helpers/DetailsHelper";
import HistoryBlockMessage from "./HistoryBlockMessage";
import HistoryBlockItemList from "./HistoryBlockItemList";
import Loaders from "@docspace/common/components/Loaders";

const History = ({
  t,
  selection,
  setSelection,
  personal,
  culture,
  getItemIcon,

  getHistory,
  openFileAction,
}) => {
  const [history, setHistory] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const parseHistoryJSON = async (fetchedHistory) => {
    let feeds = fetchedHistory.feeds;
    let newFeeds = [];
    for (let i = 0; i < feeds.length; i++) {
      const feedsJSON = JSON.parse(feeds[i].json);
      feedsJSON.author = await getUser(feedsJSON.AuthorId);

      let newGroupedFeeds = [];
      if (feeds[i].groupedFeeds) {
        let groupFeeds = feeds[i].groupedFeeds;
        for (let j = 0; j < groupFeeds.length; j++) {
          const groupFeedsJSON = JSON.parse(groupFeeds[j].json);
          newGroupedFeeds.push(groupFeedsJSON);
        }
      }

      newFeeds.push({
        ...feeds[i],
        json: feedsJSON,
        groupedFeeds: newGroupedFeeds,
      });
    }

    return { ...fetchedHistory, feeds: newFeeds };
  };

  const fetchHistory = async (itemId) => {
    let module = "files";
    if (selection.isRoom) module = "rooms";
    else if (selection.isFolder) module = "folders";

    let timerId;
    if (history) timerId = setTimeout(() => setShowLoader(true), 1500);

    let fetchedHistory = await getHistory(module, itemId);
    fetchedHistory = await parseHistoryJSON(fetchedHistory);

    clearTimeout(timerId);

    console.log(fetchedHistory);

    setHistory(fetchedHistory);
    setSelection({ ...selection, history: fetchedHistory });
    setShowLoader(false);
  };

  useEffect(async () => {
    if (selection.history) {
      setHistory(selection.history);
      return;
    }
    fetchHistory(selection.id);
  }, [selection]);

  if (!history || showLoader)
    return <Loaders.InfoPanelViewLoader view="history" />;
  return (
    <>
      <StyledHistoryList>
        <StyledHistorySubtitle>{t("RecentActivities")}</StyledHistorySubtitle>

        {history.feeds.map((feed) => (
          <StyledHistoryBlock key={feed.json.Id}>
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
                t={t}
                items={[feed.json, ...feed.groupedFeeds]}
                getItemIcon={getItemIcon}
                openFileAction={openFileAction}
              />
            </div>
          </StyledHistoryBlock>
        ))}
      </StyledHistoryList>
    </>
  );
};

export default History;
