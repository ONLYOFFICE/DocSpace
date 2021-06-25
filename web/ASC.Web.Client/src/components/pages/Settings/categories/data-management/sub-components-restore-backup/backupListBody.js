import React from "react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

import ListRow from "files/ListRow";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
const BackupListBody = ({
  displayType,
  needRowSelection,
  filesList,
  children,
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
          children={children}
        />
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
