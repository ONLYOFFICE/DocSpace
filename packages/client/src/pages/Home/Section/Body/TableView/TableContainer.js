import React, { useEffect, useRef, useCallback, useMemo } from "react";
import elementResizeDetectorMaker from "element-resize-detector";
import TableContainer from "@docspace/components/table-container";
import { inject, observer } from "mobx-react";
import { useNavigate, useLocation } from "react-router-dom";
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

const elementResizeDetector = elementResizeDetectorMaker({
  strategy: "scroll",
  callOnAdd: false,
});

const Table = ({
  filesList,
  sectionWidth,
  viewAs,
  setViewAs,
  setFirsElemChecked,
  setHeaderBorder,
  theme,
  infoPanelVisible,
  fetchMoreFiles,
  hasMoreFiles,
  filterTotal,
  isRooms,
  isTrashFolder,
  withPaging,
  columnStorageName,
  columnInfoPanelStorageName,
  highlightFile,
}) => {
  const [tagCount, setTagCount] = React.useState(null);
  const [hideColumns, setHideColumns] = React.useState(false);

  const ref = useRef(null);
  const tagRef = useRef(null);

  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const width = window.innerWidth;

    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;
    // 400 - it is desktop info panel width
    if (
      (width < 1025 && !infoPanelVisible) ||
      ((width < 625 || (viewAs === "row" && width < 1025)) &&
        infoPanelVisible) ||
      isMobile
    ) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  useEffect(() => {
    return () => {
      if (!tagRef?.current) return;

      elementResizeDetector.uninstall(tagRef.current);
    };
  }, []);

  const onResize = useCallback(
    (node) => {
      const element = tagRef?.current ? tagRef?.current : node;

      if (element) {
        const { width } = element.getBoundingClientRect();

        const columns = Math.floor(width / 100);

        if (columns != tagCount) setTagCount(columns);
      }
    },
    [tagCount]
  );

  const onSetTagRef = useCallback((node) => {
    if (node) {
      tagRef.current = node;
      onResize(node);

      elementResizeDetector.listenTo(node, onResize);
    }
  }, []);

  React.useEffect(() => {
    if (!isRooms) setTagCount(0);
  }, [isRooms]);

  const filesListNode = useMemo(() => {
    return filesList.map((item, index) => (
      <TableRow
        id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
        key={
          item?.version ? `${item.id}_${item.version}` : `${item.id}_${index}`
        }
        item={item}
        itemIndex={index}
        index={index}
        setFirsElemChecked={setFirsElemChecked}
        setHeaderBorder={setHeaderBorder}
        theme={theme}
        tagCount={tagCount}
        isRooms={isRooms}
        isTrashFolder={isTrashFolder}
        hideColumns={hideColumns}
        isHighlight={
          highlightFile.id == item.id && highlightFile.isExst === !item.fileExst
        }
      />
    ));
  }, [
    filesList,
    setFirsElemChecked,
    setHeaderBorder,
    theme,
    tagCount,
    isRooms,
    hideColumns,
    highlightFile.id,
    highlightFile.isExst,
    isTrashFolder,
  ]);

  return (
    <StyledTableContainer useReactWindow={!withPaging} forwardedRef={ref}>
      <TableHeader
        sectionWidth={sectionWidth}
        containerRef={ref}
        tagRef={onSetTagRef}
        setHideColumns={setHideColumns}
        navigate={navigate}
        location={location}
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
        itemHeight={49}
      >
        {filesListNode}
      </TableBody>
    </StyledTableContainer>
  );
};

export default inject(({ filesStore, treeFoldersStore, auth, tableStore }) => {
  const { isVisible: infoPanelVisible } = auth.infoPanelStore;

  const { isRoomsFolder, isArchiveFolder, isTrashFolder } = treeFoldersStore;
  const isRooms = isRoomsFolder || isArchiveFolder;

  const { columnStorageName, columnInfoPanelStorageName } = tableStore;

  const {
    filesList,
    viewAs,
    setViewAs,
    setFirsElemChecked,
    setHeaderBorder,
    fetchMoreFiles,
    hasMoreFiles,
    filterTotal,
    roomsFilterTotal,
    highlightFile,
  } = filesStore;

  const { withPaging, theme } = auth.settingsStore;

  return {
    filesList,
    viewAs,
    setViewAs,
    setFirsElemChecked,
    setHeaderBorder,
    theme,
    userId: auth.userStore.user?.id,
    infoPanelVisible,
    fetchMoreFiles,
    hasMoreFiles,
    filterTotal: isRooms ? roomsFilterTotal : filterTotal,
    isRooms,
    isTrashFolder,
    withPaging,
    columnStorageName,
    columnInfoPanelStorageName,
    highlightFile,
  };
})(observer(Table));
