import React from "react";
import Info from "../Info";
import History from "./History";
import Members from "./Members";

const Room = ({
  selection,
  roomState,

  defaultProps,
  membersProps,
  historyProps,
  detailsProps,
}) => {
  return roomState === "members" ? (
    <Members selectedItem={selection} {...defaultProps} {...membersProps} />
  ) : roomState === "history" ? (
    <History selectedItem={selection} {...defaultProps} {...historyProps} />
  ) : (
    <Info selection={selection} {...defaultProps} {...detailsProps} />
  );
};

export default Room;
