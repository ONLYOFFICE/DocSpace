import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import elementResizeDetectorMaker from "element-resize-detector";

import { isMobile } from "react-device-detect";

import TableContainer from "@appserver/components/table-container";
import TableBody from "@appserver/components/table-container/TableBody";

import TableHeaderContent from "./sub-components/TableHeader";
import Row from "./sub-components/TableRow";
import { Base } from "@appserver/components/themes";

const TABLE_VERSION = "1";
const TABLE_COLUMNS = `roomsTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `roomsColumnsSize_ver-${TABLE_VERSION}`;

const tagMaxWidth = 100;
const gridGap = 4;

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
    .room-name_cell {
      ${fileNameCss}
    }

    .table-container_row-context-menu-wrapper {
      ${contextCss}
    }

    .table-container_cell {
      cursor: pointer;
      background: ${(props) =>
        `${props.theme.filesSection.tableView.row.backgroundActive} !important`};

      margin-top: -1px;
      border-top: ${(props) =>
        `1px solid ${props.theme.filesSection.tableView.row.borderColor}`};
    }
  }

  .table-row-selected + .table-row-selected {
    .room-name_cell,
    .table-container_row-context-menu-wrapper {
      margin-top: -1px;
      border-image-slice: 1;
      border-top: 1px solid;
    }
    .room-name_cell {
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
`;

StyledTableContainer.defaultProps = { theme: Base };

const VirtualRoomsTable = ({
  viewAs,
  setViewAs,
  userId,
  rooms,
  sectionWidth,
  theme,
  getRoomsContextOptions,
}) => {
  const [tagCount, setTagCount] = React.useState(null);

  const containerRef = React.useRef(null);
  const firstRowRef = React.useRef(null);

  React.useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  React.useEffect(() => {
    if (!firstRowRef?.current) return;

    onResize();

    const elementResizeDetector = elementResizeDetectorMaker({
      strategy: "scroll",
      callOnAdd: false,
    });

    elementResizeDetector.listenTo(firstRowRef.current, onResize);

    return () => {
      if (!firstRowRef?.current) return;

      elementResizeDetector.uninstall(firstRowRef.current);
    };
  }, [firstRowRef, rooms]);

  const onResize = React.useCallback(() => {
    if (firstRowRef?.current) {
      const { width } = firstRowRef.current.getBoundingClientRect();

      const widthWithoutPadding = width;

      const columns = Math.floor(widthWithoutPadding / (tagMaxWidth + gridGap));

      if (columns != tagCount) setTagCount(columns);
    }
  }, [firstRowRef, tagCount]);

  const tableColumns = `${TABLE_COLUMNS}=${userId}`;
  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;

  return (
    <StyledTableContainer forwardedRef={containerRef}>
      <TableHeaderContent
        sectionWidth={sectionWidth}
        containerRef={containerRef}
        tableStorageName={tableColumns}
        columnStorageName={columnStorageName}
      />
      <TableBody>
        {rooms.map((item, index) =>
          index === 0 ? (
            <Row
              key={item.id}
              item={item}
              theme={theme}
              ref={firstRowRef}
              tagCount={tagCount}
              getContextModel={getRoomsContextOptions}
            />
          ) : (
            <Row
              key={item.id}
              item={item}
              theme={theme}
              tagCount={tagCount}
              getContextModel={getRoomsContextOptions}
            />
          )
        )}
      </TableBody>
    </StyledTableContainer>
  );
};

export default inject(
  ({ auth, filesStore, roomsStore, contextOptionsStore }) => {
    const { settingsStore } = auth;

    const { theme } = settingsStore;

    const { viewAs, setViewAs } = filesStore;

    const { getRoomsContextOptions } = contextOptionsStore;

    const { rooms } = roomsStore;

    return {
      theme,
      viewAs,
      setViewAs,
      getRoomsContextOptions,
      userId: auth.userStore.user.id,
      rooms,
    };
  }
)(observer(VirtualRoomsTable));
