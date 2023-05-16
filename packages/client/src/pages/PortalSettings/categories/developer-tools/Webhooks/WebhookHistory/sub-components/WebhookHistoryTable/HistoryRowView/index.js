import React, { useEffect } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";
import { useParams } from "react-router-dom";

import RowContainer from "@docspace/components/row-container";
import HistoryRow from "./HistoryRow";

const StyledRowContainer = styled(RowContainer)`
  margin-top: 11px;

  .row-list-item {
    cursor: pointer;
    &:hover {
      background-color: #f3f4f4;
    }
  }

  .row-list-item:has(.selected-row-item) {
    background-color: #f3f4f4;
  }
`;

const HistoryRowView = (props) => {
  const {
    historyItems,
    sectionWidth,
    viewAs,
    setViewAs,
    hasMoreItems,
    totalItems,
    fetchMoreItems,
    historyFilters,
  } = props;
  const { id } = useParams();

  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  const fetchMoreFiles = () => {
    const params = historyFilters === null ? {} : formatFilters(historyFilters);
    fetchMoreItems({ ...params, configId: id, count: 10 });
  };

  return (
    <StyledRowContainer
      filesLength={historyItems.length}
      fetchMoreFiles={fetchMoreFiles}
      hasMoreFiles={hasMoreItems}
      itemCount={totalItems}
      draggable
      useReactWindow={true}
      itemHeight={59}>
      {historyItems.map((item) => (
        <HistoryRow key={item.id} historyItem={item} sectionWidth={sectionWidth} />
      ))}
    </StyledRowContainer>
  );
};

export default inject(({ setup, webhooksStore }) => {
  const { viewAs, setViewAs } = setup;
  const { historyItems, fetchMoreItems, hasMoreItems, totalItems, historyFilters } = webhooksStore;
  return {
    viewAs,
    setViewAs,
    historyItems,
    fetchMoreItems,
    hasMoreItems,
    totalItems,
    historyFilters,
  };
})(observer(HistoryRowView));
