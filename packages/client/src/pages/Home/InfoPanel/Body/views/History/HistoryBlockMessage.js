import React from "react";

import { StyledHistoryBlockMessage } from "../../styles/history";

const HistoryBlockMessage = ({ t, action, groupedActions }) => {
  const added = t("Added");
  const created = t("Created");
  const newLabel = t("New");

  const intoThe = t("IntoThe");
  const inLabel = t("In");

  const file = t("File");
  const files = t("Files");
  const folder = t("Folder");
  const folders = t("Folders");
  const room = t("Room");

  let res = [];

  const count = groupedActions.length + 1;

  if (action.Action === 0) {
    res = [added];
    if (action.Item === "file") {
      if (count === 1) {
        res = [...res, file, intoThe, folder, `«${action.ExtraLocation}»`];
      } else {
        res = [
          ...res,
          count,
          newLabel,
          files,
          intoThe,
          folder,
          `«${action.ExtraLocation}»`,
        ];
      }
    } else if (action.Item === "folder") {
      res = [created];
      if (count === 1) {
        res = [...res, folder, inLabel, `«${action.ExtraLocation}»`];
      } else {
        res = [
          ...res,
          count,
          newLabel,
          folders,
          inLabel,
          `«${action.ExtraLocation}»`,
        ];
      }
    } else if (action.Item === "room") {
      res = [created, room, `«${action.Title}»`];
    }
  }

  return <StyledHistoryBlockMessage>{res.join(" ")}</StyledHistoryBlockMessage>;

  // return <StyledHistoryMessageContent />;
};

export default HistoryBlockMessage;
