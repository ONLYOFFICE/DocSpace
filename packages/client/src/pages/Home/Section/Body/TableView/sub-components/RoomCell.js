import { Loader, Tooltip } from "@docspace/components";
import Text from "@docspace/components/text";
import React, { useState } from "react";
import { StyledText } from "./CellStyles";
import { getFolderPath } from "@docspace/common/api/files";
import { CategoryType } from "@docspace/client/src/helpers/constants";

const RoomCell = ({ sideColor, item }) => {
  const { originRoomTitle, originId, originTitle } = item;

  const [path, setPath] = useState([]);
  const [isTooltipLoading, setIsTooltipLoading] = useState(false);

  const getPath = async () => {
    if (path.length) return;

    setIsTooltipLoading(true);
    try {
      const folderPath = await getFolderPath(originId);
      if (folderPath[0].id === CategoryType.Shared) folderPath.shift();
      setPath(folderPath);
    } catch (e) {
      console.error(e);
      setPath([{ id: 0, title: originRoomTitle || originTitle }]);
    }
    setIsTooltipLoading(false);
  };

  const canVisibleTitle = originRoomTitle || originTitle;

  return [
    <StyledText
      key="cell"
      fontSize={canVisibleTitle ? "12px" : "13px"}
      fontWeight={600}
      color={sideColor}
      className="row_update-text"
      truncate
      data-tooltip-id={"" + item.id}
      data-tip={""}
    >
      {originRoomTitle || originTitle || "—"}
    </StyledText>,

    <Tooltip
      float
      place="bottom"
      key={"tooltip"}
      id={"" + item.id}
      afterShow={getPath}
      getContent={() => (
        <span>
          {isTooltipLoading ? (
            <Loader color="#333333" size="12px" type="track" />
          ) : (
            path.map((pathPart, i) => (
              <Text key={pathPart.id} isBold={i === 0} isInline fontSize="12px">
                {i === 0 ? pathPart.title : `/${pathPart.title}`}
              </Text>
            ))
          )}
        </span>
      )}
    />,
  ];
};

export default RoomCell;
