import React, { useEffect, useRef } from "react";
import TableContainer from "@appserver/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "./TableHeader";
import TableBody from "@appserver/components/table-container/TableBody";
import { isMobile } from "react-device-detect";
import styled, { css } from "styled-components";
import { isTablet } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

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
}) => {
  const ref = useRef(null);

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

  const tableColumns = `${TABLE_COLUMNS}=${userId}`;
  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;
  const columnInfoPanelStorageName = `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;

  return (
    <StyledTableContainer forwardedRef={ref}>
      <TableHeader
        sectionWidth={sectionWidth}
        containerRef={ref}
        tableStorageName={tableColumns}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
      />
      <TableBody>
        {filesList.map((item, index) => (
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
          />
        ))}
      </TableBody>
    </StyledTableContainer>
  );
};

export default inject(({ filesStore, auth }) => {
  const { isVisible: infoPanelVisible } = auth.infoPanelStore;

  const {
    filesList,
    viewAs,
    setViewAs,
    setFirsElemChecked,
    setHeaderBorder,
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
  };
})(observer(Table));
