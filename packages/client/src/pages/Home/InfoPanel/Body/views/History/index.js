import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import {
  StyledHistoryBlock,
  StyledHistoryList,
  StyledHistorySubtitle,
} from "../../styles/history";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import { parseAndFormatDate } from "../../helpers/DetailsHelper";
import HistoryBlockMessage from "./HistoryBlockMessage";
import HistoryBlockItemList from "./HistoryBlockItemList";
import Loaders from "@docspace/common/components/Loaders";
import HistoryBlockUser from "./HistoryBlockUser";
import { FeedItemTypes } from "@docspace/common/constants";

const History = ({
  t,
  personal,
  culture,
  selection,
  selectedFolder,
  selectionParentRoom,
  setSelection,
  getInfoPanelItemIcon,
  getHistory,
  checkAndOpenLocationAction,
  openUser,
  isVisitor,
}) => {
  const [history, setHistory] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const fetchHistory = async (itemId) => {
    let module = "files";
    if (selection.isRoom) module = "rooms";
    else if (selection.isFolder) module = "folders";

    let timerId = setTimeout(() => setShowLoader(true), 1500);
    let fetchedHistory = await getHistory(module, itemId);
    fetchedHistory = parseHistoryJSON(fetchedHistory);
    clearTimeout(timerId);

    setHistory(fetchedHistory);
    setSelection({ ...selection, history: fetchedHistory });
    setShowLoader(false);
  };

  const parseHistoryJSON = (fetchedHistory) => {
    let feeds = fetchedHistory.feeds;
    let newFeeds = [];
    for (let i = 0; i < feeds.length; i++) {
      const feedsJSON = JSON.parse(feeds[i].json);

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

      newFeeds.push({
        ...feeds[i],
        json: feedsJSON,
        groupedFeeds: newGroupedFeeds,
      });
    }

    return { ...fetchedHistory, feeds: newFeeds };
  };

  useEffect(async () => {
    if (selection.history) {
      setHistory(selection.history);
      return;
    }
    fetchHistory(selection.id);
  }, [selection]);

  if (showLoader) return <Loaders.InfoPanelViewLoader view="history" />;
  if (!history) return null;

  return (
    <>
      <StyledHistoryList>
        <StyledHistorySubtitle>{t("RecentActivities")}</StyledHistorySubtitle>

        {history.feeds.map((feed) => (
          <StyledHistoryBlock
            key={feed.json.Id}
            isUserAction={feed.json.Item === FeedItemTypes.User && feed.target}
          >
            <Avatar
              role="user"
              className="avatar"
              size="min"
              source={
                feed.initiator.avatarSmall ||
                (feed.initiator.displayName
                  ? ""
                  : feed.initiator.email && "/static/images/@.react.svg")
              }
              userName={feed.initiator.displayName}
            />
            <div className="info">
              <div className="title">
                <Text className="name">{feed.initiator.displayName}</Text>
                {feed.initiator.isOwner && (
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
                className="message"
                action={feed.json}
                groupedActions={feed.groupedFeeds}
                selection={selection}
                selectedFolder={selectedFolder}
                selectionParentRoom={selectionParentRoom}
              />

              {(feed.json.Item === FeedItemTypes.File ||
                feed.json.Item === FeedItemTypes.Folder) && (
                <HistoryBlockItemList
                  t={t}
                  items={[feed.json, ...feed.groupedFeeds]}
                  getInfoPanelItemIcon={getInfoPanelItemIcon}
                  checkAndOpenLocationAction={checkAndOpenLocationAction}
                />
              )}

              {feed.json.Item === FeedItemTypes.User &&
                feed.target &&
                [feed.target, ...feed.groupedFeeds].map((user, i) => (
                  <HistoryBlockUser
                    isVisitor={isVisitor}
                    key={user.id}
                    user={user}
                    withComma={i !== feed.groupedFeeds.length}
                    openUser={openUser}
                  />
                ))}
            </div>
          </StyledHistoryBlock>
        ))}
      </StyledHistoryList>
    </>
  );
};

export default inject(({ auth, filesStore, filesActionsStore }) => {
  const { userStore } = auth;
  const {
    selection,
    selectionParentRoom,
    setSelection,
    getInfoPanelItemIcon,
    openUser,
  } = auth.infoPanelStore;
  const { personal, culture } = auth.settingsStore;

  const { getHistory } = filesStore;
  const { checkAndOpenLocationAction } = filesActionsStore;

  const { user } = userStore;
  const isVisitor = user.isVisitor;

  return {
    personal,
    culture,
    selection,
    selectionParentRoom,
    setSelection,
    getInfoPanelItemIcon,
    getHistory,
    checkAndOpenLocationAction,
    openUser,
    isVisitor,
  };
})(withTranslation(["InfoPanel", "Common", "Translations"])(observer(History)));
