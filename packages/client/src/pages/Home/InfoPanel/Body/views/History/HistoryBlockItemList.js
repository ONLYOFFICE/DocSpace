import React, { useState } from "react";
import { Trans } from "react-i18next";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import { ReactSVG } from "react-svg";
import {
  StyledHistoryBlockFile,
  StyledHistoryBlockFilesList,
} from "../../styles/history";
import { RoomsType } from "@docspace/common/constants";

export const HistoryBlockItemList = ({
  t,
  items,
  getInfoPanelItemIcon,
  checkAndOpenLocationAction,
}) => {
  const [isShowMore, setIsShowMore] = useState(items.length <= 3);
  const onShowMore = () => setIsShowMore(true);

  const parsedItems = items.map((item) => {
    const splitTitle = item.Title.split(".");
    return {
      ...item,
      isRoom: item.Item === "room",
      isFolder: item.Item === "folder",
      roomType: RoomsType[item.AdditionalInfo],
      title: splitTitle[0],
      fileExst: splitTitle[1] ? `.${splitTitle[1]}` : null,
      id: item.ItemId.split("_")[0],
      viewUrl: item.itemId,
    };
  });

  return (
    <StyledHistoryBlockFilesList>
      {parsedItems.map((item, i) => {
        if (!isShowMore && i > 2) return null;
        return (
          <StyledHistoryBlockFile isRoom={item.isRoom} key={i}>
            <ReactSVG className="icon" src={getInfoPanelItemIcon(item, 24)} />
            <div className="item-title">
              <span className="name">{item.title}</span>
              {item.fileExst && <span className="exst">{item.fileExst}</span>}
            </div>
            <IconButton
              className="location-btn"
              iconName="/static/images/folder-location.react.svg"
              size="16"
              isFill={true}
              onClick={() => checkAndOpenLocationAction(item)}
              title="Open Location"
            />
          </StyledHistoryBlockFile>
        );
      })}
      {!isShowMore && (
        <Text className="show_more-link" onClick={onShowMore}>
          <Trans
            t={t}
            ns="InfoPanel"
            i18nKey="AndMoreLabel"
            values={{ count: items.length - 3 }}
            components={{ bold: <strong /> }}
          />
        </Text>
      )}
    </StyledHistoryBlockFilesList>
  );
};

export default HistoryBlockItemList;
