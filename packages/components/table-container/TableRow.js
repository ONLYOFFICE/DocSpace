import React, { useRef } from "react";
import PropTypes from "prop-types";
import { StyledTableRow } from "./StyledTableContainer";
import TableCell from "./TableCell";
import ContextMenu from "../context-menu";
import ContextMenuButton from "../context-menu-button";

const TableRow = (props) => {
  const {
    fileContextClick,
    onHideContextMenu,
    children,
    contextOptions,
    className,
    style,
    selectionProp,
    title,
    getContextModel,
    ...rest
  } = props;

  const cm = useRef();
  const row = useRef();

  const onContextMenu = (e) => {
    fileContextClick && fileContextClick(e.button === 2);
    if (cm.current && !cm.current.menuRef.current) {
      row.current.click(e);
    }
    cm.current.show(e);
  };

  const renderContext =
    Object.prototype.hasOwnProperty.call(props, "contextOptions") &&
    contextOptions.length > 0;

  const getOptions = () => {
    fileContextClick && fileContextClick();
    return contextOptions;
  };

  return (
    <StyledTableRow
      onContextMenu={onContextMenu}
      className={`${className} table-container_row`}
      {...rest}
    >
      {children}
      <div>
        <TableCell
          {...selectionProp}
          style={style}
          forwardedRef={row}
          className={`${selectionProp?.className} table-container_row-context-menu-wrapper`}
        >
          <ContextMenu
            onHide={onHideContextMenu}
            ref={cm}
            model={contextOptions}
            getContextModel={getContextModel}
            withBackdrop={true}
          ></ContextMenu>
          {renderContext ? (
            <ContextMenuButton
              className="expandButton"
              getData={getOptions}
              directionX="right"
              displayType="toggle"
              onClick={onContextMenu}
              onClose={onHideContextMenu}
              title={title}
            />
          ) : (
            <div className="expandButton"> </div>
          )}
        </TableCell>
      </div>
    </StyledTableRow>
  );
};

TableRow.propTypes = {
  fileContextClick: PropTypes.func,
  children: PropTypes.any,
  contextOptions: PropTypes.array,
  onHideContextMenu: PropTypes.func,
  selectionProp: PropTypes.object,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
  title: PropTypes.string,
  getContextModel: PropTypes.func,
};

export default TableRow;
