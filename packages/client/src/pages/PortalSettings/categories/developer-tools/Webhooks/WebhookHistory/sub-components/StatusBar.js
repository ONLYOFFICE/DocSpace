import React, { useEffect } from "react";
import moment from "moment";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import SelectedItem from "@docspace/components/selected-item";
import Link from "@docspace/components/link";

const StatusBarWrapper = styled.div`
  margin-top: 9px;

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
        label={
          moment(historyFilters.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(historyFilters.deliveryFrom).format("HH:mm") +
          " - " +
          moment(historyFilters.deliveryTo).format("HH:mm")
        }
        onClose={clearDate}
        onClick={clearDate}
      />
    );
  };

  const SelectedDate = () => (
    <SelectedItem
      label={moment(historyFilters.deliveryDate).format("DD MMM YYYY")}
      onClose={clearDate}
      onClick={clearDate}
    />
  );

  const SelectedStatuses = historyFilters.status.map((statusCode) => (
    <SelectedItem
      label={statusCode}
      key={statusCode}
      onClose={() => unselectStatus(statusCode)}
      onClick={() => unselectStatus(statusCode)}
    />
  ));

  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format("YYYY-MM-D HH:mm") === secondDate.format("YYYY-MM-D HH:mm");
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
      {((historyFilters.deliveryDate !== null && historyFilters.status.length > 0) ||
        historyFilters.status.length > 1) && (
        <Link
          type="action"
          fontWeight={600}
          isHovered={true}
          onClick={clearAll}
          color="#A3A9AE"
          className="statusActionItem">
          Clear all
        </Link>
      )}
    </StatusBarWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { formatFilters, historyFilters, clearHistoryFilters, clearDate, unselectStatus } =
    webhooksStore;

  return { formatFilters, historyFilters, clearHistoryFilters, clearDate, unselectStatus };
})(observer(StatusBar));
