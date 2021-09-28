import React from "react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

import ListRow from "files/ListRow";
import Link from "@appserver/components/link";

import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import TrashIcon from "../../../../../../../../../public/images/button.trash.react.svg";
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
    const fileExst = "gz";
    const modifyFileName = fileName.substring(0, fileName.indexOf("gz"));
    return (
      <div style={style}>
        <ListRow
          displayType={displayType}
          needRowSelection={needRowSelection}
          index={index}
          fileName={modifyFileName}
          fileExst={fileExst}
          noCheckBox
        >
          <div data-index={index} className="backup-list_options">
            <Link
              data-index={index}
              className="backup-list_restore-link"
              onClick={onRestoreClick}
            >
              {t("RestoreBackup")}
            </Link>
            <TrashIcon
              data-index={index}
              className="backup-list_trash-icon"
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
