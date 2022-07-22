import React from "react";

import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import Group from "./Group";

const GroupList = ({ groupList, onGroupClick, isMultiSelect }) => {
  return (
    <AutoSizer>
      {({ width, height }) => (
        <List
          className="options-list"
          height={height - 8}
          width={width + 8}
          itemCount={groupList.length}
          itemData={{ groupList, onGroupClick, isMultiSelect }}
          itemSize={48}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Group}
        </List>
      )}
    </AutoSizer>
  );
};

export default React.memo(GroupList);
