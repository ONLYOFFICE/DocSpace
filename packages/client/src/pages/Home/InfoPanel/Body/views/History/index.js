import React, { useState, useEffect, useRef } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import { StyledHistoryList, StyledHistorySubtitle } from "../../styles/history";

import Loaders from "@docspace/common/components/Loaders";
import { getRelativeDateDay } from "./../../helpers/HistoryHelper";
import HistoryBlock from "./HistoryBlock";
import NoHistory from "../NoItem/NoHistory";
import { toastr } from "@docspace/components";

const HISTORY_LOAD = 1500;
const HISTORY_TIMEOUT = 30000;

const History = ({
  t,
  selection,
  selectedFolder,
  selectionHistory,
  setSelectionHistory,
  selectionParentRoom,
  getInfoPanelItemIcon,
  getHistory,
  checkAndOpenLocationAction,
  openUser,
  isVisitor,
  setView,
  isCollaborator,
}) => {
  let history = selectionHistory;
  const [showLoader, setShowLoader] = useState(false);

  const isMount = useRef(true);

  const fetchHistory = async (itemId) => {
    let module = "files";
    if (selection.isRoom) module = "rooms";
    else if (selection.isFolder) module = "folders";

    let loadTimerId = setTimeout(() => setShowLoader(true), HISTORY_LOAD);
    let timeoutTimerId = setTimeout(() => {
      toastr.error(`History load timeout of ${HISTORY_TIMEOUT}ms exceeded`);
      setView("info_details");
    }, HISTORY_TIMEOUT);

    getHistory(module, itemId)
      .then((data) => {
        const parsedHistory = parseHistoryJSON(data);
        if (isMount.current) setHistory(parsedHistory);
      })
      .finally(() => {
        clearTimeout(loadTimerId);
        clearTimeout(timeoutTimerId);
        if (isMount.current) setShowLoader(false);
      });
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

  useEffect(() => {
    if (!isMount.current) return;
    fetchHistory(selection.id);
  }, [selection]);

  useEffect(() => {
    return () => (isMount.current = false);
  }, []);

  if (showLoader) return <Loaders.InfoPanelViewLoader view="history" />;
  if (!history) return <></>;
  if (history?.feeds?.length === 0) return <NoHistory t={t} />;

  return (
    <>
      <StyledHistoryList>
        {history.feedsByDays.map(({ day, feeds }) => [
          <StyledHistorySubtitle key={day}>{day}</StyledHistorySubtitle>,
          ...feeds.map((feed, i) => (
            <HistoryBlock
              t={t}
              key={feed.json.Id}
              selectionIsFile={history?.isFile}
              feed={feed}
              selection={selection}
              selectedFolder={selectedFolder}
              selectionParentRoom={selectionParentRoom}
              getInfoPanelItemIcon={getInfoPanelItemIcon}
              checkAndOpenLocationAction={checkAndOpenLocationAction}
              openUser={openUser}
              isVisitor={isVisitor}
              isCollaborator={isCollaborator}
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
    selectionHistory,
    setSelectionHistory,
    getInfoPanelItemIcon,
    openUser,
    setView,
  } = auth.infoPanelStore;
  const { personal, culture } = auth.settingsStore;

  const { getHistory } = filesStore;
  const { checkAndOpenLocationAction } = filesActionsStore;

  const { user } = userStore;
  const isVisitor = user.isVisitor;
  const isCollaborator = user.isCollaborator;

  return {
    personal,
    culture,
    selection,
    selectionParentRoom,
    selectionHistory,
    setSelectionHistory,
    getInfoPanelItemIcon,
    getHistory,
    setView,
    checkAndOpenLocationAction,
    openUser,
    isVisitor,
    isCollaborator,
  };
})(withTranslation(["InfoPanel", "Common", "Translations"])(observer(History)));
