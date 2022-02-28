import React from "react";
import PropTypes from "prop-types";
import Text from "../text";
import IconButton from "../icon-button";
import globalColors from "../utils/globalColors";
import { StyledTableHeaderCell } from "./StyledTableContainer";

const TableHeaderCell = ({
  column,
  index,
  onMouseDown,
  resizable,
  sortBy,
  sorted,
  defaultSize,
  sortingVisible,
}) => {
  const { title, enable, active, minWidth } = column;

  const isActive = (sortBy && column.sortBy === sortBy) || active;

  const onClick = (e) => {
    if (!sortingVisible) return;
    column.onClick && column.onClick(column.sortBy, e);
  };

  const onIconClick = (e) => {
    if (!sortingVisible) return;
    column.onIconClick();
    e.stopPropagation();
  };

  return (
    <StyledTableHeaderCell
      sorted={sorted}
      isActive={isActive}
      showIcon={column.onClick}
      className="table-container_header-cell"
      id={`column_${index}`}
      data-enable={enable}
      data-min-width={minWidth}
      data-default-size={defaultSize}
      onClick={onClick}
      sortingVisible={sortingVisible}
    >
      <div className="table-container_header-item">
        <div className="header-container-text-wrapper">
          <Text
            fontWeight={600}
            color={isActive ? globalColors.grayMain : globalColors.gray}
            className="header-container-text"
          >
            {enable ? title : ""}
          </Text>

          {sortingVisible && (
            <IconButton
              onClick={column.onIconClick ? onIconClick : onClick}
              iconName="/static/images/sort.desc.react.svg"
              className="header-container-text-icon"
              size="small"
              hoverColor="#657077"
              color={isActive ? globalColors.grayMain : globalColors.gray}
            />
          )}
        </div>
        {resizable && (
          <div
            data-column={`${index}`}
            className="resize-handle not-selectable"
            onMouseDown={onMouseDown}
          />
        )}
      </div>
    </StyledTableHeaderCell>
  );
};

TableHeaderCell.propTypes = {
  column: PropTypes.object,
  index: PropTypes.number,
  onMouseDown: PropTypes.func,
  resizable: PropTypes.bool,
  sorted: PropTypes.bool,
  sortBy: PropTypes.string,
  defaultSize: PropTypes.number,
  sortingVisible: PropTypes.bool,
};

export default TableHeaderCell;
