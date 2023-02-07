import React, { useState, useEffect, useRef } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import { StyledHistoryList, StyledHistorySubtitle } from "../../styles/history";

import Loaders from "@docspace/common/components/Loaders";
import { getRelativeDateDay } from "./../../helpers/HistoryHelper";
import HistoryBlock from "./HistoryBlock";

const History = ({
  t,
  selection,
  selectedFolder,
  selectionParentRoom,
  setSelection,
  getInfoPanelItemIcon,
  getHistory,
  checkAndOpenLocationAction,
  openUser,
  isVisitor,
  searchTitleOpenLocation,
  itemOpenLocation,
  isLoadedSearchFiles,
  getFolderInfo,
  getFileInfo,
  setSelectionFiles,
}) => {
  const [history, setHistory] = useState(null);
  const [showLoader, setShowLoader] = useState(false);

  const isMount = useRef(true);

  useEffect(() => {
    return () => (isMount.current = false);
  }, []);

  useEffect(() => {
    if (!(searchTitleOpenLocation && isLoadedSearchFiles && itemOpenLocation))
      return;

    const requestInfo = itemOpenLocation.isFolder ? getFolderInfo : getFileInfo;

    requestInfo(+itemOpenLocation.id).then((res) => {
      if (itemOpenLocation.isFolder) res.isFolder = true;

      setSelectionFiles([res]);
      setSelection(res);
    });
  }, [
    searchTitleOpenLocation,
    isLoadedSearchFiles,
    itemOpenLocation,
    getFolderInfo,
    getFileInfo,
    setSelectionFiles,
  ]);

  const fetchHistory = async (itemId) => {
    let module = "files";
    if (selection.isRoom) module = "rooms";
    else if (selection.isFolder) module = "folders";

    let timerId = setTimeout(() => setShowLoader(true), 1500);
    let fetchedHistory = await getHistory(module, itemId);
    fetchedHistory = parseHistoryJSON(fetchedHistory);
    clearTimeout(timerId);

    if (isMount.current) {
      setHistory(fetchedHistory);
      setSelection({ ...selection, history: fetchedHistory });
      setShowLoader(false);
    }
  };

  const parseHistoryJSON = (fetchedHistory) => {
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

    return { ...fetchedHistory, feedsByDays: parsedFeeds };
  };

  useEffect(async () => {
    if (!isMount.current) return;

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
        {history.feedsByDays.map(({ day, feeds }) => [
          <StyledHistorySubtitle key={day}>{day}</StyledHistorySubtitle>,
          ...feeds.map((feed, i) => (
            <HistoryBlock
              key={feed.json.Id}
              t={t}
              feed={feed}
              selection={selection}
              selectedFolder={selectedFolder}
              selectionParentRoom={selectionParentRoom}
              getInfoPanelItemIcon={getInfoPanelItemIcon}
              checkAndOpenLocationAction={checkAndOpenLocationAction}
              openUser={openUser}
              isVisitor={isVisitor}
              isLastEntity={i === feeds.length - 1}
            />
          )),
        ])}
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

  const {
    getHistory,
    getFolderInfo,
    getFileInfo,
    setSelection: setSelectionFiles,
  } = filesStore;
  const {
    checkAndOpenLocationAction,
    searchTitleOpenLocation,
    itemOpenLocation,
    isLoadedSearchFiles,
  } = filesActionsStore;

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
    searchTitleOpenLocation,
    itemOpenLocation,
    isLoadedSearchFiles,
    getFolderInfo,
    getFileInfo,
    setSelectionFiles,
  };
})(withTranslation(["InfoPanel", "Common", "Translations"])(observer(History)));
