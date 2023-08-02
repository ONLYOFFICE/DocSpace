import React, { useEffect, useRef } from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { useNavigate, useLocation } from "react-router-dom";

import TableContainer from "@docspace/components/table-container";
import TableBody from "@docspace/components/table-container/TableBody";

import EmptyScreen from "../EmptyScreen";

import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import { Base } from "@docspace/components/themes";
import { TableVersions } from "SRC_DIR/helpers/constants";

const COLUMNS_SIZE = `peopleColumnsSize_ver-${TableVersions.Accounts}`;
const INFO_PANEL_COLUMNS_SIZE = `infoPanelPeopleColumnsSize_ver-${TableVersions.Accounts}`;

const marginCss = css`
  margin-top: -1px;
  border-top: ${(props) =>
    `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
`;

const userNameCss = css`
  margin-left: -24px;
  padding-left: 24px;
  ${marginCss}
`;

const contextCss = css`
  margin-right: -20px;
  padding-right: 18px;
  ${marginCss}
`;

const StyledTableContainer = styled(TableContainer)`
  .table-row-selected {
    .table-container_user-name-cell {
      ${userNameCss}
    }

    .table-container_row-context-menu-wrapper {
      ${contextCss}
    }
  }

  .table-row-selected + .table-row-selected {
    .table-row {
      .table-container_user-name-cell,
      .table-container_row-context-menu-wrapper {
        margin-top: -1px;
        border-image-slice: 1;
        border-top: 1px solid;
      }
      .table-container_user-name-cell {
        ${userNameCss}
        border-left: 0; //for Safari macOS
        border-right: 0; //for Safari macOS

        border-image-source: ${(props) => `linear-gradient(to right, 
          ${props.theme.filesSection.tableView.row.borderColorTransition} 17px, ${props.theme.filesSection.tableView.row.borderColor} 31px)`};
      }
      .table-container_row-context-menu-wrapper {
        ${contextCss}

        border-image-source: ${(props) => `linear-gradient(to left,
          ${props.theme.filesSection.tableView.row.borderColorTransition} 17px, ${props.theme.filesSection.tableView.row.borderColor} 31px)`};
      }
    }
  }

  .user-item:not(.table-row-selected) + .table-row-selected {
    .table-row {
      .table-container_user-name-cell {
        ${userNameCss}
      }

      .table-container_row-context-menu-wrapper {
        ${contextCss}
      }

      .table-container_user-name-cell,
      .table-container_row-context-menu-wrapper {
        border-bottom: ${(props) =>
          `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
      }
    }
  }
`;

StyledTableContainer.defaultProps = { theme: Base };

const Table = ({
  peopleList,
  sectionWidth,
  accountsViewAs,
  setViewAs,
  theme,
  isAdmin,
  isOwner,
  changeType,
  userId,
  infoPanelVisible,

  fetchMoreAccounts,
  hasMoreAccounts,
  filterTotal,
  withPaging,
  canChangeUserType,
  isFiltered,
}) => {
  const ref = useRef(null);
  const [hideColumns, setHideColumns] = React.useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const width = window.innerWidth;

    if (
      accountsViewAs !== "tile" &&
      ((accountsViewAs !== "table" && accountsViewAs !== "row") ||
        !sectionWidth)
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

  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;
  const columnInfoPanelStorageName = `${INFO_PANEL_COLUMNS_SIZE}=${userId}`;

  return peopleList.length !== 0 || !isFiltered ? (
    <StyledTableContainer useReactWindow={!withPaging} forwardedRef={ref}>
      <TableHeader
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        containerRef={ref}
        setHideColumns={setHideColumns}
        navigate={navigate}
        location={location}
      />
      <TableBody
        infoPanelVisible={infoPanelVisible}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        columnStorageName={columnStorageName}
        fetchMoreFiles={fetchMoreAccounts}
        hasMoreFiles={hasMoreAccounts}
        itemCount={filterTotal}
        filesLength={peopleList.length}
        itemHeight={49}
        useReactWindow={!withPaging}
      >
        {peopleList.map((item, index) => (
          <TableRow
            theme={theme}
            key={item.id}
            item={item}
            isAdmin={isAdmin}
            isOwner={isOwner}
            changeUserType={changeType}
            userId={userId}
            canChangeUserType={canChangeUserType}
            hideColumns={hideColumns}
            itemIndex={index}
          />
        ))}
      </TableBody>
    </StyledTableContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(
  ({ peopleStore, auth, accessRightsStore, filesStore }) => {
    const {
      usersStore,
      filterStore,
      viewAs: accountsViewAs,
      setViewAs,
      changeType,
    } = peopleStore;
    const { theme, withPaging } = auth.settingsStore;
    const { peopleList, hasMoreAccounts, fetchMoreAccounts } = usersStore;
    const { filterTotal, isFiltered } = filterStore;

    const { isVisible: infoPanelVisible } = auth.infoPanelStore;
    const { isAdmin, isOwner, id: userId } = auth.userStore.user;

    const { canChangeUserType } = accessRightsStore;

    return {
      peopleList,
      accountsViewAs,
      setViewAs,
      theme,
      isAdmin,
      isOwner,
      changeType,
      userId,
      infoPanelVisible,
      withPaging,

      fetchMoreAccounts,
      hasMoreAccounts,
      filterTotal,
      canChangeUserType,
      isFiltered,
    };
  }
)(observer(Table));
