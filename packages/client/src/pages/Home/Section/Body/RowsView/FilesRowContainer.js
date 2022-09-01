import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@docspace/components/row-container";
import SimpleFilesRow from "./SimpleFilesRow";
import { isMobile } from "react-device-detect";
import styled from "styled-components";
import marginStyles from "./CommonStyles";
import { isTablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

const StyledRowContainer = styled(RowContainer)`
  .row-selected + .row-wrapper:not(.row-selected) {
    .files-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }

  .row-wrapper:not(.row-selected)
    //+ .row-wrapper:not(.row-hotkey-border)
    + .row-selected {
    .files-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }

  .row-hotkey-border + .row-selected {
    .files-row {
      border-top: 1px solid #2da7db !important;
    }
  }

  .row-selected:last-child {
    .files-row {
      border-bottom: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      padding-bottom: 1px;
      ${marginStyles}
    }
    .files-row::after {
      height: 0px;
    }
  }
  .row-selected:first-child {
    .files-row {
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }
`;

StyledRowContainer.defaultProps = { theme: Base };

const FilesRowContainer = ({
  filesList,
  sectionWidth,
  viewAs,
  setViewAs,
  infoPanelVisible,
  filterTotal,
  fetchMoreFiles,
  hasMoreFiles,
  isRooms,
  selectedFolderId,
  withPaging,
}) => {
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

  return (
    <StyledRowContainer
      className="files-row-container"
      filesLength={filesList.length}
      itemCount={filterTotal}
      fetchMoreFiles={fetchMoreFiles}
      hasMoreFiles={hasMoreFiles}
      draggable
      useReactWindow={!withPaging}
      selectedFolderId={selectedFolderId}
      itemHeight={58}
    >
      {filesList.map((item, index) => (
        <SimpleFilesRow
          id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
          key={`${item.id}_${index}`}
          item={item}
          sectionWidth={sectionWidth}
          isRooms={isRooms}
        />
      ))}
    </StyledRowContainer>
  );
};

export default inject(
  ({ filesStore, auth, treeFoldersStore, selectedFolderStore }) => {
    const {
      filesList,
      viewAs,
      setViewAs,
      filterTotal,
      fetchMoreFiles,
      hasMoreFiles,
      withPaging,
      roomsFilterTotal,
    } = filesStore;
    const { isVisible: infoPanelVisible } = auth.infoPanelStore;
    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    return {
      filesList,
      viewAs,
      setViewAs,
      infoPanelVisible,
      filterTotal: isRooms ? roomsFilterTotal : filterTotal,
      fetchMoreFiles,
      hasMoreFiles,
      isRooms,
      selectedFolderId: selectedFolderStore.id,
      withPaging,
    };
  }
)(observer(FilesRowContainer));
