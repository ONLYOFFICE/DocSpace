import React, { useRef } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import { withTranslation } from "react-i18next";
import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";
import AuthorCell from "./sub-components/AuthorCell";
import DateCell from "./sub-components/DateCell";
import globalColors from "@appserver/components/utils/globalColors";
import styled from "styled-components";

const sideColor = globalColors.gray;

const StyledShare = styled.div`
  cursor: pointer;

  .share-button {
    padding: 4px;
    border: 1px solid transparent;
    border-radius: 3px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    :hover {
      border: 1px solid #a3a9ae;
      svg {
        cursor: pointer;
      }
    }

    .share-button-icon {
      margin-right: 7px;
    }
  }
`;

const StyledBadgesContainer = styled.div`
  display: flex;
  align-items: center;
  height: 19px;
  margin-left: 8px;

  .badges {
    display: flex;
    align-items: center;
    height: 19px;
  }

  .badge {
    cursor: pointer;
    height: 14px;
    width: 14px;
    margin-right: 6px;
  }
`;

const FilesTableRow = (props) => {
  const {
    contextOptionsProps,
    fileContextClick,
    element,
    item,
    onContentFileSelect,
    checkedProps,
    className,
    value,
    onMouseClick,
    badgesComponent,
  } = props;

  const selectionProp = {
    className: `files-item ${className}`,
    value,
  };

  return (
    <TableRow
      selectionProp={selectionProp}
      key={item.id}
      item={item}
      element={element}
      fileContextClick={fileContextClick}
      onContentFileSelect={onContentFileSelect}
      onClick={onMouseClick}
      {...contextOptionsProps}
      {...checkedProps}
    >
      <TableCell {...selectionProp}>
        <FileNameCell {...props} />
        <StyledBadgesContainer>{badgesComponent}</StyledBadgesContainer>
      </TableCell>
      <TableCell {...selectionProp}>
        <AuthorCell sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp}>
        <DateCell create sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp}>
        <DateCell sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp}>
        <SizeCell sideColor={sideColor} {...props} />
      </TableCell>

      <TableCell {...selectionProp}>TYPE?</TableCell>

      <TableCell {...selectionProp}>
        <StyledShare>{props.sharedButton}</StyledShare>
      </TableCell>
    </TableRow>
  );
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))
  )
);
