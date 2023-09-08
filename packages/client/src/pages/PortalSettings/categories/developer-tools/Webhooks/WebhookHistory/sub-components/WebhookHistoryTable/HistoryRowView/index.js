import React, { useEffect } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { isMobile, isMobileOnly } from "react-device-detect";
import { useParams } from "react-router-dom";

import RowContainer from "@docspace/components/row-container";
import HistoryRow from "./HistoryRow";

import { Base } from "@docspace/components/themes";

const StyledRowContainer = styled(RowContainer)`
  margin-top: 11px;

  .row-list-item {
    cursor: pointer;
    padding-inline-end: ${() => (isMobileOnly ? "5px" : "15px")};
  }
  .row-item::after {
    bottom: -3px;
  }

  .row-list-item:has(.selected-row-item) {
    background-color: ${(props) =>
      props.theme.isBase ? "#f3f4f4" : "#282828"};
  }
`;

StyledRowContainer.defaultProps = { theme: Base };

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
    formatFilters,
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
    fetchMoreItems({ ...params, configId: id });
  };

  return (
    <StyledRowContainer
      filesLength={historyItems.length}
      fetchMoreFiles={fetchMoreFiles}
      hasMoreFiles={hasMoreItems}
      itemCount={totalItems}
      draggable
      useReactWindow={true}
      itemHeight={59}
    >
      {historyItems.map((item) => (
        <HistoryRow
          key={item.id}
          historyItem={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </StyledRowContainer>
  );
};

export default inject(({ setup, webhooksStore }) => {
  const { viewAs, setViewAs } = setup;
  const {
    historyItems,
    fetchMoreItems,
    hasMoreItems,
    totalItems,
    historyFilters,
    formatFilters,
  } = webhooksStore;
  return {
    viewAs,
    setViewAs,
    historyItems,
    fetchMoreItems,
    hasMoreItems,
    totalItems,
    historyFilters,
    formatFilters,
  };
})(observer(HistoryRowView));
