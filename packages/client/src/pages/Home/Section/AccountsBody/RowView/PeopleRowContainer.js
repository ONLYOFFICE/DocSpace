import React, { useEffect } from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import RowContainer from "@docspace/components/row-container";

import EmptyScreen from "../EmptyScreen";

import SimpleUserRow from "./SimpleUserRow";
import withLoader from "SRC_DIR/HOCs/withLoader";

const marginStyles = css`
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  ${isMobile &&
  css`
    margin-left: -20px;
    margin-right: -20px;
    padding-left: 20px;
    padding-right: 20px;
  `}

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }

  @media (max-width: 375px) {
    margin-left: -16px;
    margin-right: -8px;
    padding-left: 16px;
    padding-right: 8px;
  }
`;

const StyledRowContainer = styled(RowContainer)`
  .row-selected + .row-wrapper:not(.row-selected) {
    .user-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;

      ${marginStyles}
    }
  }

  .row-wrapper:not(.row-selected) + .row-selected {
    .user-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;

      ${marginStyles}
    }
  }

  .row-hotkey-border + .row-selected {
    .user-row {
      border-top: 1px solid #2da7db !important;
    }
  }

  .row-selected:last-child {
    .user-row {
      border-bottom: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      padding-bottom: 1px;

      ${marginStyles}
    }
    .user-row::after {
      height: 0px;
    }
  }
  .row-selected:first-child {
    .user-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;

      ${marginStyles}
    }
  }
`;

const PeopleRowContainer = ({
  peopleList,
  sectionWidth,
  accountsViewAs,
  setViewAs,
  theme,
  infoPanelVisible,
  isFiltered,
  fetchMoreAccounts,
  hasMoreAccounts,
  filterTotal,
  withPaging,
}) => {
  useEffect(() => {
    const width = window.innerWidth;

    if (
      (accountsViewAs !== "table" && accountsViewAs !== "row") ||
      !sectionWidth
    )
      return;
    // 400 - it is desktop info panel width
    if (
      (width < 1025 && !infoPanelVisible) ||
      ((width < 625 || (accountsViewAs === "row" && width < 1025)) &&
        infoPanelVisible) ||
      isMobile
    ) {
      accountsViewAs !== "row" && setViewAs("row");
    } else {
      accountsViewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return peopleList.length !== 0 || !isFiltered ? (
    <StyledRowContainer
      className="people-row-container"
      useReactWindow={!withPaging}
      fetchMoreFiles={fetchMoreAccounts}
      hasMoreFiles={hasMoreAccounts}
      itemCount={filterTotal}
      filesLength={peopleList.length}
      itemHeight={58}
    >
      {peopleList.map((item, index) => (
        <SimpleUserRow
          theme={theme}
          key={item.id}
          item={item}
          itemIndex={index}
          sectionWidth={sectionWidth}
        />
      ))}
    </StyledRowContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore, auth, filesStore }) => {
  const {
    usersStore,
    filterStore,
    viewAs: accountsViewAs,
    setViewAs,
  } = peopleStore;
  const { theme, withPaging } = auth.settingsStore;
  const { peopleList, hasMoreAccounts, fetchMoreAccounts } = usersStore;
  const { filterTotal, isFiltered } = filterStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;

  return {
    peopleList,
    accountsViewAs,
    setViewAs,
    theme,
    infoPanelVisible,
    withPaging,

    fetchMoreAccounts,
    hasMoreAccounts,
    filterTotal,
    isFiltered,
  };
})(observer(PeopleRowContainer));
