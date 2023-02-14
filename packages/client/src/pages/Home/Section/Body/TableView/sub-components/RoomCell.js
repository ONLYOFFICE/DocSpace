import { Tooltip } from "@docspace/components";
import Text from "@docspace/components/text";
import React from "react";
import { StyledText } from "./CellStyles";

const RoomCell = ({ t, sideColor, item }) => {
  const { originRoomTitle, originTitle } = item;

  const roomTitle = originRoomTitle || originTitle;
  const path = originRoomTitle ? originTitle : null;

  return [
    <StyledText
      key="cell"
      // title={roomTitle}
      fontSize="12px"
      fontWeight={600}
      color={sideColor}
      className="row_update-text"
      truncate
      data-for={"" + item.id}
      data-tip={""}
      data-place={"bottom"}
    >
      {roomTitle}
    </StyledText>,

    <Tooltip
      id={"" + item.id}
      key="tooltip"
      getContent={() => (
        <div>
          <Text isBold isInline fontSize="12px">
            {roomTitle}
          </Text>
          {path && (
            <Text isInline fontSize="12px">
              {`/${path}`}
            </Text>
          )}
        </div>
      )}
    />,
  ];
};

export default RoomCell;
