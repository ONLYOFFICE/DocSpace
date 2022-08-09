import React from "react";
import Details from "./Details";
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

  return <></>;

  // if (selectedItems.length === 0)
  //   return <Details {...defaultProps} {...detailsProps} />;
  // else return <Details {...defaultProps} {...detailsProps} />;
};

export default Room;
