import React, { useEffect } from "react";
import moment from "moment";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { SelectedItem } from "@docspace/components";
import Link from "@docspace/components/link";

const StatusBarWrapper = styled.div`
  margin-top: 8px;

  .statusBarItem {
    margin-right: 4px;
  }

  .statusBarItem:last-of-type {
    margin-right: 0;
  }

  .statusActionItem {
    margin-left: 12px;
  }
`;

const StatusBar = (props) => {
  const {
    historyFilters,
    formatFilters,
    applyFilters,
    clearHistoryFilters,
    clearDate,
    unselectStatus,
  } = props;

  const clearAll = () => {
    applyFilters(
      formatFilters({
        deliveryDate: null,
        status: [],
      }),
    );
    clearHistoryFilters();
  };

  const SelectedDateTime = () => {
    return (
      <SelectedItem
        onClose={clearDate}
        text={
          moment(historyFilters.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(historyFilters.deliveryFrom).format("HH:mm") +
          " - " +
          moment(historyFilters.deliveryTo).format("HH:mm")
        }
        className="statusBarItem"
      />
    );
  };

  const SelectedDate = () => (
    <SelectedItem
      onClose={clearDate}
      text={moment(historyFilters.deliveryDate).format("DD MMM YYYY")}
      className="statusBarItem"
    />
  );

  const SelectedStatuses = historyFilters.status.map((statusCode) => (
    <SelectedItem
      onClose={() => unselectStatus(statusCode)}
      text={statusCode}
      key={statusCode}
      className="statusBarItem"
    />
  ));

  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format() === secondDate.format();
  };

  useEffect(() => {
    applyFilters(formatFilters(historyFilters));
    if (historyFilters.deliveryDate === null && historyFilters.status.length === 0) {
      clearHistoryFilters();
    }
  }, [historyFilters]);

  return historyFilters.deliveryDate === null && historyFilters.status.length === 0 ? (
    ""
  ) : (
    <StatusBarWrapper>
      {historyFilters.deliveryDate !== null ? (
        !isEqualDates(
          historyFilters.deliveryFrom,
          historyFilters.deliveryFrom.clone().startOf("day"),
        ) ||
        !isEqualDates(historyFilters.deliveryTo, historyFilters.deliveryTo.clone().endOf("day")) ? (
          <SelectedDateTime />
        ) : (
          <SelectedDate />
        )
      ) : (
        ""
      )}
      {SelectedStatuses}
      <Link
        type="action"
        fontWeight={600}
        isHovered={true}
        onClick={clearAll}
        color="#A3A9AE"
        className="statusActionItem">
        Clear all
      </Link>
    </StatusBarWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { formatFilters, historyFilters, clearHistoryFilters, clearDate, unselectStatus } =
    webhooksStore;

  return { formatFilters, historyFilters, clearHistoryFilters, clearDate, unselectStatus };
})(observer(StatusBar));
