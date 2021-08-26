import React, { useRef } from "react";
import PropTypes from "prop-types";
import { StyledTableRow } from "./StyledTableContainer";
import TableCell from "./TableCell";
import ContextMenu from "../context-menu";
import ContextMenuButton from "../context-menu-button";
import Checkbox from "../checkbox";

const TableRow = (props) => {
  const {
    fileContextClick,
    children,
    contextOptions,
    checked,
    element,
    onContentSelect,
    item,
    className,
    style,
    selectionProp,
    ...rest
  } = props;

  const cm = useRef();
  const row = useRef();

  const onContextMenu = (e) => {
    fileContextClick && fileContextClick();
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

  const onChange = (e) => {
    onContentSelect && onContentSelect(e.target.checked, item);
  };

  console.log("selectionProp", selectionProp);

  return (
    <StyledTableRow
      onContextMenu={onContextMenu}
      className={`${className} table-container_row`}
      {...rest}
    >
      <TableCell
        checked={checked}
        {...selectionProp}
        style={style}
        className={`${selectionProp?.className} table-container_row-checkbox-wrapper`}
      >
        <div className="table-container_element">{element}</div>
        <Checkbox
          className="table-container_row-checkbox"
          onChange={onChange}
          isChecked={checked}
        />
      </TableCell>
      {children}
      <div>
        <TableCell {...selectionProp} style={style} forwardedRef={row}>
          <ContextMenu ref={cm} model={contextOptions}></ContextMenu>
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
        </TableCell>
      </div>
    </StyledTableRow>
  );
};

TableRow.propTypes = {
  fileContextClick: PropTypes.func,
  children: PropTypes.any,
  contextOptions: PropTypes.array,
  checked: PropTypes.bool,
  element: PropTypes.any,
  onContentSelect: PropTypes.func,
  item: PropTypes.object,
  selectionProp: PropTypes.object,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
};

export default TableRow;
