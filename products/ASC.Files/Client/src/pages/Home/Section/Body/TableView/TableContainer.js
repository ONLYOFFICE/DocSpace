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

const Table = ({
  filesList,
  sectionWidth,
  viewAs,
  setViewAs,
  setFirsElemChecked,
  setHeaderBorder,
  theme,
  infoPanelVisible,
}) => {
  const ref = useRef(null);

  useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;

    if (
      (sectionWidth < 1025 && !infoPanelVisible) ||
      (isTablet() && infoPanelVisible) ||
      isMobile
    ) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <StyledTableContainer forwardedRef={ref}>
      <TableHeader sectionWidth={sectionWidth} containerRef={ref} />
      <TableBody>
        {filesList.map((item, index) => (
          <TableRow
            key={`${item.id}_${index}`}
            item={item}
            index={index}
            setFirsElemChecked={setFirsElemChecked}
            setHeaderBorder={setHeaderBorder}
            theme={theme}
          />
        ))}
      </TableBody>
    </StyledTableContainer>
  );
};

export default inject(({ filesStore, infoPanelStore, auth }) => {
  const { isVisible: infoPanelVisible } = infoPanelStore;

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
    infoPanelVisible,
  };
})(observer(Table));
