import React from "react";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import BookmarksProps from "./Bookmarks.props";
import { Item, List, Text } from "./Bookmarks.styled";

function Bookmarks({ bookmarks, navigate }: BookmarksProps) {
  return (
    <CustomScrollbarsVirtualList>
      <List>
        {bookmarks.map((item, index) => {
          return (
            <Item key={index}>
              <Text onClick={() => navigate(index)}>{item.description}</Text>
            </Item>
          );
        })}
      </List>
    </CustomScrollbarsVirtualList>
  );
}

export default Bookmarks;
