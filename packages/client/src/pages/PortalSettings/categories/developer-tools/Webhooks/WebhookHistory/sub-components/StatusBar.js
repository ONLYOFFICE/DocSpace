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
  const { statusFilters, setStatusFilters, formatFilters, applyFilters } = props;

  const clearDate = () => {
    setStatusFilters((prevStatusFilters) => ({ ...prevStatusFilters, deliveryDate: null }));
  };

  const unselectStatus = (statusCode) => {
    setStatusFilters((prevStatusFilters) => ({
      ...prevStatusFilters,
      status: prevStatusFilters.status.filter((item) => item !== statusCode),
    }));
  };

  const clearAll = () => {
    applyFilters(
      formatFilters({
        deliveryDate: null,
        status: [],
      }),
    );
    setStatusFilters(null);
  };

  const SelectedDateTime = () => {
    return (
      <SelectedItem
        onClose={clearDate}
        text={
          moment(statusFilters.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(statusFilters.deliveryFrom).format("HH:mm") +
          " - " +
          moment(statusFilters.deliveryTo).format("HH:mm")
        }
        className="statusBarItem"
      />
    );
  };

  const SelectedDate = () => (
    <SelectedItem
      onClose={clearDate}
      text={moment(statusFilters.deliveryDate).format("DD MMM YYYY")}
      className="statusBarItem"
    />
  );

  const SelectedStatuses = statusFilters.status.map((statusCode) => (
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
    applyFilters(formatFilters(statusFilters));
    if (statusFilters.deliveryDate === null && statusFilters.status.length === 0) {
      setStatusFilters(null);
    }
  }, [statusFilters]);

  return statusFilters.deliveryDate === null && statusFilters.status.length === 0 ? (
    ""
  ) : (
    <StatusBarWrapper>
      {statusFilters.deliveryDate !== null ? (
        !isEqualDates(
          statusFilters.deliveryFrom,
          statusFilters.deliveryFrom.clone().startOf("day"),
        ) ||
        !isEqualDates(statusFilters.deliveryTo, statusFilters.deliveryTo.clone().endOf("day")) ? (
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
  const { formatFilters } = webhooksStore;

  return { formatFilters };
})(observer(StatusBar));
