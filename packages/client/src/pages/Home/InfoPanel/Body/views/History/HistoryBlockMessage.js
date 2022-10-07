import React from "react";
import { Trans } from "react-i18next";

import { FeedActionTypes, FeedItemTypes } from "@docspace/common/constants";

import { StyledHistoryBlockMessage } from "../../styles/history";

const HistoryBlockMessage = ({
  t,
  action,
  groupedActions,
  selection,
  selectedFolder,
  selectionParentRoom,
}) => {
  const getTranslationKey = () => {
    const getActionType = () => {
      switch (action.Action) {
        case FeedActionTypes.Create:
          return "Create";
        case FeedActionTypes.Update:
          return "Update";
        case FeedActionTypes.Rename:
          return "Rename";
        case FeedActionTypes.Move:
          return "Move";
        case FeedActionTypes.Delete:
          return "Delete";
      }
    };

    const getActionItem = () => {
      switch (action.Item) {
        case FeedItemTypes.File:
          return "File";
        case FeedItemTypes.Folder:
          return "Folder";
        case FeedItemTypes.Room:
          return "Room";
        case FeedItemTypes.User:
          return "User";
      }
    };

    const getActionCount = () => {
      if (
        !(
          action.Action === FeedActionTypes.Create ||
          action.Action === FeedActionTypes.Delete
        )
      )
        return "";

      switch (action.Item) {
        case "file":
          return !groupedActions.length ? "Single" : "Several";
        case "folder":
          return !groupedActions.length ? "Single" : "Several";
        default:
          return "";
      }
    };

    let res = "Feed";
    res += getActionType();
    res += getActionItem();
    res += getActionCount();
    return res;
  };

  const getData = () => {
    switch (action.Item) {
      case FeedItemTypes.Room:
        return { roomTitle: action.Title, oldRoomTitle: "" };
      default:
        return {};
    }
  };

  const getFolderLabel = () => {
    if (action.Item !== "file" && action.Item !== "folder") return "";

    const itemLocationId = +action.ExtraLocation;
    if (selectedFolder.id === itemLocationId) return "";
    // if (selectionParentRoom?.id === itemLocation) return "";

    const folderTitle = action.ExtraLocationTitle;
    if (!folderTitle) return "";

    return (
      <span className="folderLabel">
        {` ${t("FeedLocationLabel", { folderTitle })}`}
      </span>
    );
  };

  return (
    <StyledHistoryBlockMessage className="message">
      <Trans
        t={t}
        ns="InfoPanel"
        i18nKey={getTranslationKey()}
        values={getData()}
        components={{ bold: <strong /> }}
      />
      {getFolderLabel()}
    </StyledHistoryBlockMessage>
  );
};

export default HistoryBlockMessage;
