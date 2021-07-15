import React, { useRef } from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import { withTranslation } from "react-i18next";
import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";
import CheckboxCell from "./sub-components/CheckboxCell";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";
import AuthorCell from "./sub-components/AuthorCell";
import CreatedCell from "./sub-components/CreatedCell";
import globalColors from "@appserver/components/utils/globalColors";
import styled from "styled-components";

import ContextMenu from "@appserver/components/context-menu";
import ContextMenuButton from "@appserver/components/context-menu-button";

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
  const { contextOptionsProps, fileContextClick } = props;

  const cm = useRef();
  const row = useRef();

  const onContextMenu = (e) => {
    props.rowContextClick && props.rowContextClick();
    if (cm.current && !cm.current.menuRef.current) {
      row.current.click(e); //TODO: need fix context menu to global
    }
    cm.current.show(e);
  };

  const renderContext =
    Object.prototype.hasOwnProperty.call(props, "contextOptionsProps") &&
    props.contextOptionsProps.contextOptions.length > 0;

  const getOptions = () => {
    props.rowContextClick && props.rowContextClick();
    return props.contextOptionsProps.contextOptions;
  };

  const style = props.index === 0 ? { style: { marginTop: 40 } } : {};

  return (
    <TableRow rowContextClick={fileContextClick} {...contextOptionsProps}>
      <CheckboxCell index={props.index} {...props} />

      <TableCell {...style}>
        <FileNameCell index={props.index} {...props} />
      </TableCell>
      <TableCell {...style}>
        <AuthorCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...style}>
        <CreatedCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...style}>
        <SizeCell index={props.index} sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell {...style}>
        <StyledShare index={props.index}> {props.sharedButton}</StyledShare>
      </TableCell>
      <TableCell {...style} forwardedRef={row} onContextMenu={onContextMenu}>
        <div index={props.index}>
          <ContextMenu
            ref={cm}
            model={props.contextOptionsProps.contextOptions}
          ></ContextMenu>
          {renderContext ? (
            <ContextMenuButton
              color="#A3A9AE"
              hoverColor="#657077"
              className="expandButton"
              getData={getOptions}
              directionX="right"
              isNew={true}
              onClick={onContextMenu}
            />
          ) : (
            <div className="expandButton"> </div>
          )}
        </div>
      </TableCell>
    </TableRow>
  );
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))
  )
);
