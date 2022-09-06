import React from "react";
import Details from "../Details";
import History from "../History";
import Members from "../Members";

const Room = ({
  roomState,

  selection,
  setSelection,

  defaultProps,
  membersProps,
  historyProps,
  detailsProps,
}) => {
  return roomState === "members" ? (
    <Members
      selection={selection}
      setSelection={setSelection}
      {...defaultProps}
      {...membersProps}
    />
  ) : roomState === "history" ? (
    <History selectedItem={selection} {...defaultProps} {...historyProps} />
  ) : (
    <Details selection={selection} {...defaultProps} {...detailsProps} />
  );
};

export default Room;
