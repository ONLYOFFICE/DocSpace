import React from "react";
import { Trans } from "react-i18next";

import { FeedActionTypes, FeedItemTypes } from "@docspace/common/constants";

import { StyledHistoryBlockMessage } from "../../styles/history";
import getBlockMessageTranslation from "./HistroryBlockMessageTranslations";

const HistoryBlockMessage = ({
  t,
  action,
  groupedActions,
  selection,
  selectedFolder,
  selectionParentRoom,
}) => {
  const message = getBlockMessageTranslation(
    t,
    action.Action,
    action.Item,
    action.Item === FeedItemTypes.File || action.Item === FeedItemTypes.Folder
      ? !!groupedActions.length
      : false,
    action.Item === FeedItemTypes.Room
      ? { roomTitle: action.Title, oldRoomTitle: "" }
      : {}
  );

  const getFolderLabel = () => {
    if (action.Item !== "file" && action.Item !== "folder") return "";

    const itemLocationId = +action.ExtraLocation;
    if (selectedFolder?.id === itemLocationId) return "";
    if (selection?.isRoom && selectionParentRoom?.id === itemLocationId)
      return "";

    const folderTitle = action.ExtraLocationTitle;
    if (!folderTitle) return "";

    return (
      <span className="folder-label">
        {` ${t("FeedLocationLabel", { folderTitle })}`}
      </span>
    );
  };

  return (
    <StyledHistoryBlockMessage className="message">
      <span className="main-message">{message}</span>
      {getFolderLabel()}
    </StyledHistoryBlockMessage>
  );
};

export default HistoryBlockMessage;
