import React from "react";

import { FeedActions } from "@docspace/common/constants";

import { StyledHistoryBlockMessage } from "../../styles/history";

const HistoryBlockMessage = ({ t, action, groupedActions }) => {
  const getActionType = () => {
    return FeedActions[action.Action];
    // switch (action.Action) {
    //   case 0:
    //     return "Create";
    //   case 1:
    //     return "Update";
    //   default:
    //     return "Create";
    // }
  };

  const getActionItem = () => {
    switch (action.Item) {
      case "file":
        return "File";
      case "folder":
        return "Folder";
      case "room":
        return "Room";
      case "sharedRoom":
        return "User";
      default:
        return "File";
    }
  };

  const getFolderLabel = () => {
    const folderTitle = action.ExtraLocation;
    if (!folderTitle || (action.Item !== "file" && action.Item !== "folder"))
      return "";
    return t("FeedLocationLabel", { folderTitle });
  };

  let res = "Feed";
  res += getActionType();
  res += getActionItem();

  return (
    <StyledHistoryBlockMessage>
      {t(res)} {t(getFolderLabel())}
    </StyledHistoryBlockMessage>
  );

  // return <StyledHistoryMessageContent />;
};

export default HistoryBlockMessage;
