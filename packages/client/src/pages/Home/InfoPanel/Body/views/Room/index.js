import React from "react";
import Info from "../Info";
import History from "./History";
import Members from "./Members";

const Room = ({
  selectedItems,
  selectedFolder,
  roomState,

  defaultProps,
  membersProps,
  historyProps,
  detailsProps,
}) => {
  const selectedItem =
    selectedItems.length === 0
      ? selectedFolder
      : selectedItems.length === 1
      ? selectedItems[0]
      : selectedItems;

  return roomState === "members" ? (
    <Members selectedItem={selectedItem} {...defaultProps} {...membersProps} />
  ) : roomState === "history" ? (
    <History selectedItem={selectedItem} {...defaultProps} {...historyProps} />
  ) : (
    <Info selectedItem={selectedItem} {...defaultProps} {...detailsProps} />
  );
};

export default Room;
