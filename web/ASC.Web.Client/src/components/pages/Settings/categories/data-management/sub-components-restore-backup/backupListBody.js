import React from "react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

import ListRow from "files/ListRow";
import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";

import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
const BackupListBody = ({
  displayType,
  needRowSelection,
  filesList,
  onIconClick,
  onRestoreClick,
  t,
}) => {
  const Item = ({ index, style }) => {
    const file = filesList[index];
    const fileName = file.fileName;
    return (
      <div style={style}>
        <ListRow
          displayType={displayType}
          needRowSelection={needRowSelection}
          index={index}
          fileName={fileName}
        >
          <div data-index={index} className="backup-list_options">
            <Link
              data-index={index}
              className="backup-list_restore-link"
              onClick={onRestoreClick}
            >
              {t("RestoreBackup")}
            </Link>

            <IconButton
              className="backup-list_trash-icon"
              size={16}
              color="#657077"
              iconName="/static/images/button.trash.react.svg"
              onClick={onIconClick}
            />
          </div>
        </ListRow>
      </div>
    );
  };
  return (
    <div className="backup-list-row-list">
      <AutoSizer>
        {({ height, width }) => (
          <List
            height={height}
            width={width + 8}
            itemSize={displayType === "aside" ? 56 : 36}
            itemCount={filesList.length}
            itemData={filesList}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {Item}
          </List>
        )}
      </AutoSizer>
    </div>
  );
};

export default BackupListBody;
