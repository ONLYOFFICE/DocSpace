import React from "react";
import Row from "@docspace/components/row";
import { HistoryContent } from "./HistoryContent";

export const HistoryUserRow = ({ item, sectionWidth }) => {
  return (
    <Row sectionWidth={sectionWidth} key={item.id} data={item}>
      <HistoryContent item={item} sectionWidth={sectionWidth} />
    </Row>
  );
};
