import React from "react";
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
  const { filterSettings, clearFilterDate, toggleStatus, clearFilterSettings } = props;

  const SelectedDateTime = () => {
    return (
      <SelectedItem
        onClose={clearFilterDate}
        text={
          moment(filterSettings.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(filterSettings.deliveryFrom).format("HH:mm") +
          " - " +
          moment(filterSettings.deliveryTo).format("HH:mm")
        }
        className="statusBarItem"
      />
    );
  };

  const SelectedDate = () => (
    <SelectedItem
      onClose={clearFilterDate}
      text={moment(filterSettings.deliveryDate).format("DD MMM YYYY")}
      className="statusBarItem"
    />
  );

  const SelectedStatuses = filterSettings.status.map((statusCode) => (
    <SelectedItem
      onClose={() => toggleStatus(statusCode)}
      text={statusCode}
      key={statusCode}
      className="statusBarItem"
    />
  ));

  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format() === secondDate.format();
  };

  return (
    <StatusBarWrapper>
      {filterSettings.deliveryDate !== null ? (
        !isEqualDates(
          filterSettings.deliveryFrom,
          filterSettings.deliveryFrom.clone().startOf("day"),
        ) ||
        !isEqualDates(filterSettings.deliveryTo, filterSettings.deliveryTo.clone().endOf("day")) ? (
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
        onClick={clearFilterSettings}
        color="#A3A9AE"
        className="statusActionItem">
        Clear all
      </Link>
    </StatusBarWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { filterSettings, clearFilterDate, toggleStatus, clearFilterSettings } = webhooksStore;

  return { filterSettings, clearFilterDate, toggleStatus, clearFilterSettings };
})(observer(StatusBar));
