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
import CreatedCell from "./sub-components/CreatedCell";
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
  } = props;

  const style = props.index === 0 ? { style: { marginTop: 40 } } : {};

  const selectionProp = {
    className: `files-item ${className}`,
    value,
  };

  return (
    <TableRow
      selectionProp={selectionProp}
      style={style}
      key={item.id}
      item={item}
      element={element}
      fileContextClick={fileContextClick}
      onContentFileSelect={onContentFileSelect}
      {...contextOptionsProps}
      {...checkedProps}
    >
      <TableCell {...selectionProp} {...style}>
        <FileNameCell index={props.index} {...props} />
      </TableCell>
      <TableCell {...selectionProp} {...style}>
        <AuthorCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp} {...style}>
        <CreatedCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp} {...style}>
        <SizeCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...selectionProp} {...style}>
        <StyledShare index={props.index}> {props.sharedButton}</StyledShare>
      </TableCell>
    </TableRow>
  );
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))
  )
);
