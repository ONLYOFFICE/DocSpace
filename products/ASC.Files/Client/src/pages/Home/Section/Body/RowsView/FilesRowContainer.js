import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import RowContainer from "@appserver/components/row-container";
import SimpleFilesRow from "./SimpleFilesRow";
import { isMobile } from "react-device-detect";
import styled from "styled-components";
import marginStyles from "./CommonStyles";

const borderColor = "#ECEEF1";

const StyledRowContainer = styled(RowContainer)`
  .row-selected + .row-wrapper:not(.row-selected) {
    .files-row {
      border-top: ${`1px ${borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }

  .row-wrapper:not(.row-selected)
    + .row-wrapper:not(.row-hotkey-border)
    + .row-selected {
    .files-row {
      border-top: ${`1px ${borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }

  .row-selected:last-child {
    .files-row {
      border-bottom: ${`1px ${borderColor} solid`};
      padding-bottom: 1px;
      ${marginStyles}
    }
    .files-row::after {
      height: 0px;
    }
  }
  .row-selected:first-child {
    .files-row {
      border-top: ${`1px ${borderColor} solid`};
      margin-top: -3px;
      ${marginStyles}
    }
  }
`;
const FilesRowContainer = ({ filesList, sectionWidth, viewAs, setViewAs }) => {
  useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !sectionWidth) return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <StyledRowContainer
      className="files-row-container"
      draggable
      useReactWindow={false}
    >
      {filesList.map((item, index) => (
        <SimpleFilesRow
          key={`${item.id}_${index}`}
          item={item}
          sectionWidth={sectionWidth}
        />
      ))}
    </StyledRowContainer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList, viewAs, setViewAs } = filesStore;

  return {
    filesList,
    viewAs,
    setViewAs,
  };
})(observer(FilesRowContainer));
