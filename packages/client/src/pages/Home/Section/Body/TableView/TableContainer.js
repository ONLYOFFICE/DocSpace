import React, { useEffect, useRef } from "react";
import elementResizeDetectorMaker from "element-resize-detector";
import TableContainer from "@docspace/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import TableBody from "@docspace/components/table-container/TableBody";
import { isMobile } from "react-device-detect";
import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const marginCss = css`
  margin-top: -1px;
  border-top: ${(props) =>
    `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
`;

const fileNameCss = css`
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
    .table-container_file-name-cell {
      ${fileNameCss}
    }

    .table-container_row-context-menu-wrapper {
      ${contextCss}
    }
  }

  .table-row-selected + .table-row-selected {
    .table-row {
      .table-container_file-name-cell,
      .table-container_row-context-menu-wrapper {
        margin-top: -1px;
        border-image-slice: 1;
        border-top: 1px solid;
      }
      .table-container_file-name-cell {
        ${fileNameCss}
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

  .table-hotkey-border + .table-row-selected {
    .table-row {
      .table-container_file-name-cell {
        border-top: unset !important;
        margin-top: 0 !important;
      }

      .table-container_row-context-menu-wrapper {
        border-top: unset !important;
        margin-top: 0 !important;
      }
    }
  }

  .files-item:not(.table-row-selected) + .table-row-selected {
    .table-row {
      .table-container_file-name-cell {
        ${fileNameCss}
      }

      .table-container_row-context-menu-wrapper {
        ${contextCss}
      }

      .table-container_file-name-cell,
      .table-container_row-context-menu-wrapper {
        border-bottom: ${(props) =>
          `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
      }
    }
  }
`;

StyledTableContainer.defaultProps = { theme: Base };

const TABLE_VERSION = "2";
const TABLE_COLUMNS = `filesTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `filesColumnsSize_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE_INFO_PANEL = `filesColumnsSizeInfoPanel_ver-${TABLE_VERSION}`;

const TABLE_ROOMS_VERSION = "1";
const TABLE_ROOMS_COLUMNS = `roomsTableColumns_ver-${TABLE_ROOMS_VERSION}`;
const COLUMNS_ROOMS_SIZE = `roomsColumnsSize_ver-${TABLE_ROOMS_VERSION}`;
const COLUMNS_ROOMS_SIZE_INFO_PANEL = `roomsColumnsSizeInfoPanel_ver-${TABLE_ROOMS_VERSION}`;

const Table = ({
  filesList,
  sectionWidth,
  viewAs,
  setViewAs,
  setFirsElemChecked,
  setHeaderBorder,
  theme,
  infoPanelVisible,
  userId,
  fetchMoreFiles,
  hasMoreFiles,
  filterTotal,
  isRooms,
  selectedFolderId,
  withPaging,
}) => {
  const [tagCount, setTagCount] = React.useState(null);

  const ref = useRef(null);
  const tagRef = useRef(null);

  useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;
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

  React.useEffect(() => {
    if (!tagRef?.current) return;

    onResize();

    const elementResizeDetector = elementResizeDetectorMaker({
      strategy: "scroll",
      callOnAdd: false,
    });

    elementResizeDetector.listenTo(tagRef.current, onResize);

    return () => {
      if (!tagRef?.current) return;

      elementResizeDetector.uninstall(tagRef.current);
    };
  }, [tagRef, filesList]);

  const onResize = React.useCallback(() => {
    if (tagRef?.current) {
      const { width } = tagRef.current.getBoundingClientRect();

      const columns = Math.floor(width / 100);

      if (columns != tagCount) setTagCount(columns);
    }
  }, [tagRef, tagCount]);

  const tableColumns = isRooms
    ? `${TABLE_ROOMS_COLUMNS}=${userId}`
    : `${TABLE_COLUMNS}=${userId}`;
  const columnStorageName = isRooms
    ? `${COLUMNS_ROOMS_SIZE}=${userId}`
    : `${COLUMNS_SIZE}=${userId}`;
  const columnInfoPanelStorageName = isRooms
    ? `${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`
    : `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;

  return (
    <StyledTableContainer useReactWindow={!withPaging} forwardedRef={ref}>
      <TableHeader
        sectionWidth={sectionWidth}
        containerRef={ref}
        tableStorageName={tableColumns}
        columnStorageName={columnStorageName}
        filesColumnStorageName={`${COLUMNS_SIZE}=${userId}`}
        roomsColumnStorageName={`${COLUMNS_ROOMS_SIZE}=${userId}`}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        filesColumnInfoPanelStorageName={`${COLUMNS_SIZE_INFO_PANEL}=${userId}`}
        roomsColumnInfoPanelStorageName={`${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`}
        isRooms={isRooms}
      />

      <TableBody
        fetchMoreFiles={fetchMoreFiles}
        columnStorageName={columnStorageName}
        filesLength={filesList.length}
        hasMoreFiles={hasMoreFiles}
        itemCount={filterTotal}
        useReactWindow={!withPaging}
        infoPanelVisible={infoPanelVisible}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        selectedFolderId={selectedFolderId}
        itemHeight={isRooms ? 49 : 41}
      >
        {filesList.map((item, index) => {
          return index === 0 && item.isRoom ? (
            <TableRow
              id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
              key={`${item.id}_${index}`}
              item={item}
              index={index}
              setFirsElemChecked={setFirsElemChecked}
              setHeaderBorder={setHeaderBorder}
              theme={theme}
              tableColumns={tableColumns}
              columnStorageName={columnStorageName}
              columnInfoPanelStorageName={columnInfoPanelStorageName}
              tagRef={tagRef}
              tagCount={tagCount}
              isRooms={isRooms}
            />
          ) : (
            <TableRow
              id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
              key={`${item.id}_${index}`}
              item={item}
              index={index}
              setFirsElemChecked={setFirsElemChecked}
              setHeaderBorder={setHeaderBorder}
              theme={theme}
              tableColumns={tableColumns}
              columnStorageName={columnStorageName}
              columnInfoPanelStorageName={columnInfoPanelStorageName}
              tagCount={tagCount}
              isRooms={isRooms}
            />
          );
        })}
      </TableBody>
    </StyledTableContainer>
  );
};

export default inject(
  ({ filesStore, treeFoldersStore, auth, selectedFolderStore }) => {
    const { isVisible: infoPanelVisible } = auth.infoPanelStore;

    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const {
      filesList,
      viewAs,
      setViewAs,
      setFirsElemChecked,
      setHeaderBorder,
      fetchMoreFiles,
      hasMoreFiles,
      filterTotal,
      withPaging,
      roomsFilterTotal,
    } = filesStore;

    return {
      filesList,
      viewAs,
      setViewAs,
      setFirsElemChecked,
      setHeaderBorder,
      theme: auth.settingsStore.theme,
      userId: auth.userStore.user.id,
      infoPanelVisible,
      fetchMoreFiles,
      hasMoreFiles,
      filterTotal: isRooms ? roomsFilterTotal : filterTotal,
      isRooms,
      selectedFolderId: selectedFolderStore.id,
      withPaging,
    };
  }
)(observer(Table));
