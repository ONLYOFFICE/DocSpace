import React, { useEffect, useRef } from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

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
  viewAs,
  setViewAs,
  theme,
  isAdmin,
  isOwner,
  changeType,
  userId,
  infoPanelVisible,

  isLoading,
  fetchMoreAccounts,
  hasMoreAccounts,
  filterTotal,
  withPaging,
  canChangeUserType,
}) => {
  const ref = useRef(null);

  useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !sectionWidth) return;
    // 400 - it is desktop info panel width
    if (
      (sectionWidth < 1025 && !infoPanelVisible) ||
      ((sectionWidth < 625 || (viewAs === "row" && sectionWidth < 1025)) &&
        infoPanelVisible) ||
      isMobile
    ) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;
  const columnInfoPanelStorageName = `${INFO_PANEL_COLUMNS_SIZE}=${userId}`;

  if (isLoading) return <></>

  return peopleList.length > 0 ? (
    <StyledTableContainer useReactWindow={!withPaging} forwardedRef={ref}>
      <TableHeader
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        containerRef={ref}
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
            itemIndex={index}
          />
        ))}
      </TableBody>
    </StyledTableContainer>
  ) : (
    <EmptyScreen />
  );
};

export default inject(({ peopleStore, auth, accessRightsStore }) => {
  const {
    usersStore,
    filterStore,
    viewAs,
    setViewAs,
    changeType,
    loadingStore
  } = peopleStore;
  const { theme, withPaging } = auth.settingsStore;
  const { peopleList, hasMoreAccounts, fetchMoreAccounts } = usersStore;
  const { filterTotal } = filterStore;

  const { isVisible: infoPanelVisible } = auth.infoPanelStore;
  const { isAdmin, isOwner, id: userId } = auth.userStore.user;

  const { canChangeUserType } = accessRightsStore;

  const { isLoading } = loadingStore

  return {
    peopleList,
    viewAs,
    setViewAs,
    theme,
    isAdmin,
    isOwner,
    changeType,
    userId,
    infoPanelVisible,
    withPaging,
    isLoading,
    fetchMoreAccounts,
    hasMoreAccounts,
    filterTotal,
    canChangeUserType,
  };
})(observer(Table));
