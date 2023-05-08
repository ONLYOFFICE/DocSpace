import React from "react";
import styled from "styled-components";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import BookmarksProps from "./Bookmarks.props";

const List = styled.ul`
  padding-left: 16px;
  padding-right: 30px;

  list-style: none;
  margin-top: 0px;

  display: flex;
  flex-direction: column;
`;

const Item = styled.li`
  color: #ffffff;
  padding: 0 16px;
  font-weight: 400;
  font-size: 13px;
  line-height: 20px;

  cursor: pointer;

  :hover {
    background: #474747;
  }
`;

const Text = styled.p`
  margin: 0;
  border-bottom: 1px solid #474747;
  padding: 6px 0;
`;

function Bookmarks({ bookmarks, navigate }: BookmarksProps) {
  return (
    <CustomScrollbarsVirtualList>
      <List>
        {bookmarks.map((item, index) => (
          <Item key={index}>
            <Text onClick={() => navigate(index)}>{item.description}</Text>
          </Item>
        ))}
      </List>
    </CustomScrollbarsVirtualList>
  );
}

export default Bookmarks;
