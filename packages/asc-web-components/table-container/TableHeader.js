import React from "react";
import PropTypes from "prop-types";
import throttle from "lodash.throttle";
import {
  StyledTableHeader,
  StyledTableRow,
  StyledEmptyTableContainer,
} from "./StyledTableContainer";
import Checkbox from "../checkbox";
import TableSettings from "./TableSettings";
import TableHeaderCell from "./TableHeaderCell";
import { size } from "../utils/device";
import TableGroupMenu from "./TableGroupMenu";

const minColumnSize = 150;
const settingsSize = 24;

class TableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.state = { columnIndex: null };

    this.headerRef = React.createRef();
    this.throttledResize = throttle(this.onResize, 300);
  }

  componentDidMount() {
    this.onResize();
    window.addEventListener("resize", this.throttledResize);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
  }

  componentDidUpdate() {
    this.onResize();
  }

  getSubstring = (str) => +str.substring(0, str.length - 2);

  getNextColumn = (array, index) => {
    let i = 1;
    while (array.length !== i) {
      const item = array[index + i];

      if (!item) return null;
      else if (!item.enable) i++;
      else return item;
    }
  };

  getColumn = (array, index) => {
    let i = 1;
    while (array.length !== i) {
      const item = array[index + i];
      if (!item) return [0, i];
      else if (item === "0px") i++;
      else return [this.getSubstring(item), i];
    }
  };

  moveToLeft = (widths, newWidth, index) => {
    const { columnIndex } = this.state;

    let leftColumn;
    let colIndex = index ? index : columnIndex - 1;

    if (colIndex === 0) return;

    while (colIndex !== 0) {
      leftColumn = document.getElementById("column_" + colIndex);
      if (leftColumn) {
        if (leftColumn.dataset.enable === "true") break;
        else colIndex--;
      } else return false;
    }

    const minSize = leftColumn.dataset.minWidth
      ? leftColumn.dataset.minWidth
      : minColumnSize;

    if (leftColumn.clientWidth <= minSize) {
      if (colIndex === 1) return false;
      return this.moveToLeft(widths, newWidth, colIndex - 1);
    }

    const offset = this.getSubstring(widths[+columnIndex]) - newWidth;
    const column2Width = this.getSubstring(widths[colIndex]);

    const leftColumnWidth = column2Width - offset;
    const newLeftWidth = leftColumnWidth < minSize ? minSize : leftColumnWidth;

    widths[colIndex] = newLeftWidth + "px";
    widths[+columnIndex] =
      this.getSubstring(widths[+columnIndex]) +
      (offset - (newLeftWidth - leftColumnWidth)) +
      "px";
  };

  moveToRight = (widths, newWidth, index) => {
    const { columnIndex } = this.state;

    let rightColumn;
    let colIndex = index ? index : +columnIndex + 1;

    while (colIndex !== this.props.columns.length) {
      rightColumn = document.getElementById("column_" + colIndex);
      if (rightColumn) {
        if (rightColumn.dataset.enable === "true") break;
        else colIndex++;
      } else return false;
    }

    const offset = this.getSubstring(widths[+columnIndex]) - newWidth;
    const column2Width = this.getSubstring(widths[colIndex]);

    if (column2Width + offset >= minColumnSize) {
      widths[+columnIndex] = newWidth + "px";
      widths[colIndex] = column2Width + offset + "px";
    } else {
      if (colIndex === this.props.columns.length) return false;
      return this.moveToRight(widths, newWidth, colIndex + 1);
    }
  };

  addNewColumns = (gridTemplateColumns, columnIndex) => {
    const filterColumns = this.props.columns
      .filter((x) => x.enable)
      .filter((x) => x.key !== this.props.columns[columnIndex - 1].key)
      .filter((x) => !x.defaultSize);

    let index = this.props.columns.length;
    while (index !== 0) {
      index--;
      const someItem = this.props.columns[index];

      const isFind = filterColumns.find((x) => x.key === someItem.key);
      if (isFind) {
        const someItemById = document.getElementById("column_" + (index + 1));

        const columnSize = someItemById.clientWidth - minColumnSize;

        if (columnSize >= minColumnSize) {
          return (gridTemplateColumns[index + 1] = columnSize + "px");
        }
      }
    }
  };

  onMouseMove = (e) => {
    const { columnIndex } = this.state;
    const { containerRef } = this.props;
    if (!columnIndex) return;
    const column = document.getElementById("column_" + columnIndex);
    const columnSize = column.getBoundingClientRect();
    const newWidth = e.clientX - columnSize.left;

    const tableContainer = containerRef.current.style.gridTemplateColumns;
    const widths = tableContainer.split(" ");

    const minSize = column.dataset.minWidth
      ? column.dataset.minWidth
      : minColumnSize;

    if (newWidth <= minSize) {
      const columnChanged = this.moveToLeft(widths, newWidth);

      if (!columnChanged) {
        widths[+columnIndex] = widths[+columnIndex];
      }
    } else {
      this.moveToRight(widths, newWidth);
    }

    containerRef.current.style.gridTemplateColumns = widths.join(" ");
    this.headerRef.current.style.gridTemplateColumns = widths.join(" ");
  };

  onMouseUp = () => {
    localStorage.setItem(
      this.props.columnStorageName,
      this.props.containerRef.current.style.gridTemplateColumns
    );

    window.removeEventListener("mousemove", this.onMouseMove);
    window.removeEventListener("mouseup", this.onMouseUp);
  };

  onMouseDown = (event) => {
    this.setState({ columnIndex: event.target.dataset.column });

    window.addEventListener("mousemove", this.onMouseMove);
    window.addEventListener("mouseup", this.onMouseUp);
  };

  onResize = () => {
    const {
      containerRef,
      columnStorageName,
      checkboxSize,
      resetColumnsSize,
      sectionWidth,
    } = this.props;

    let activeColumnIndex = null;

    const container = containerRef.current
      ? containerRef.current
      : document.getElementById("table-container");

    const minSize = size.tablet;
    const containerMargin = 25;

    if (
      !container ||
      +container.clientWidth + containerMargin <= minSize ||
      sectionWidth <= minSize
    )
      return;

    const storageSize =
      !resetColumnsSize && localStorage.getItem(columnStorageName);
    const tableContainer = storageSize
      ? storageSize.split(" ")
      : container.style.gridTemplateColumns.split(" ");

    const containerWidth = +container.clientWidth;
    const newContainerWidth =
      containerWidth - this.getSubstring(checkboxSize) - 80 - settingsSize; // TODO: 80

    const oldWidth = tableContainer
      .map((column) => this.getSubstring(column))
      .reduce((x, y) => x + y);

    const enableColumns = this.props.columns
      .filter((x) => !x.default)
      .filter((x) => x.enable)
      .filter((x) => !x.defaultSize);

    const isSingleTable = enableColumns.length > 0;

    let str = "";
    let disableColumnWidth = 0;

    if (tableContainer.length > 1) {
      const gridTemplateColumns = [];

      for (let index in tableContainer) {
        const item = tableContainer[index];

        const column = document.getElementById("column_" + index);
        const enable =
          index == 0 ||
          index == tableContainer.length - 1 ||
          (column ? column.dataset.enable === "true" : item !== "0px");

        const isActiveNow = item === "0px" && enable;
        if (isActiveNow && column) activeColumnIndex = index;

        if (!enable) {
          gridTemplateColumns.push("0px");
          gridTemplateColumns[1] =
            this.getSubstring(gridTemplateColumns[1]) +
            this.getSubstring(item) +
            "px";
        } else if (
          item !== `${settingsSize}px` &&
          item !== checkboxSize &&
          item !== "80px"
        ) {
          const percent = (this.getSubstring(item) / oldWidth) * 100;

          if (index == 1) {
            const newItemWidth =
              (containerWidth * percent) / 100 + disableColumnWidth + "px";
            gridTemplateColumns.push(newItemWidth);
          } else {
            const newItemWidth =
              percent === 0
                ? `${minColumnSize}px`
                : (containerWidth * percent) / 100 + "px";

            gridTemplateColumns.push(newItemWidth);
          }
        } else {
          gridTemplateColumns.push(item);
        }
      }

      if (activeColumnIndex) {
        this.addNewColumns(gridTemplateColumns, activeColumnIndex);
      }

      str = gridTemplateColumns.join(" ");
    } else {
      const column =
        (newContainerWidth * (isSingleTable ? 60 : 100)) / 100 + "px";
      const percent = 40 / enableColumns.length;
      const otherColumns = (newContainerWidth * percent) / 100 + "px";

      str = `${checkboxSize} ${column} `;
      for (let col of this.props.columns) {
        if (!col.default) {
          str += col.enable
            ? col.defaultSize
              ? `${col.defaultSize}px `
              : `${otherColumns} `
            : "0px ";
        }
      }

      str += `${settingsSize}px`;
    }
    container.style.gridTemplateColumns = str;
    if (this.headerRef.current) {
      this.headerRef.current.style.gridTemplateColumns = str;
      this.headerRef.current.style.width = containerWidth + "px";
    }

    localStorage.setItem(columnStorageName, str);
  };

  onChange = (checked) => {
    this.props.setSelected(checked);
  };

  render() {
    const {
      columns,
      sortBy,
      sorted,
      isHeaderVisible,
      checkboxOptions,
      containerRef,
      onChange,
      isChecked,
      isIndeterminate,
      headerMenu,
      columnStorageName,
      hasAccess,
      ...rest
    } = this.props;

    //console.log("TABLE HEADER RENDER", columns);

    return (
      <>
        {isHeaderVisible ? (
          <TableGroupMenu
            checkboxOptions={checkboxOptions}
            containerRef={containerRef}
            onChange={onChange}
            isChecked={isChecked}
            isIndeterminate={isIndeterminate}
            headerMenu={headerMenu}
            columnStorageName={columnStorageName}
            {...rest}
          />
        ) : (
          <StyledTableHeader
            className="table-container_header"
            ref={this.headerRef}
            {...rest}
          >
            <StyledTableRow>
              {hasAccess ? (
                <Checkbox
                  className="table-container_header-checkbox"
                  onChange={this.onChange}
                  isChecked={false}
                />
              ) : (
                <div></div>
              )}

              {columns.map((column, index) => {
                const nextColumn = this.getNextColumn(columns, index);
                const resizable = nextColumn ? nextColumn.resizable : false;

                return (
                  <TableHeaderCell
                    key={column.key}
                    index={index}
                    column={column}
                    sorted={sorted}
                    sortBy={sortBy}
                    resizable={resizable}
                    onMouseDown={this.onMouseDown}
                  />
                );
              })}

              <div className="table-container_header-settings">
                <TableSettings columns={columns} />
              </div>
            </StyledTableRow>
          </StyledTableHeader>
        )}
        <StyledEmptyTableContainer />
      </>
    );
  }
}

TableHeader.defaultProps = {
  hasAccess: true,
};

TableHeader.propTypes = {
  containerRef: PropTypes.shape({ current: PropTypes.any }).isRequired,
  columns: PropTypes.array.isRequired,
  setSelected: PropTypes.func.isRequired,
  sortBy: PropTypes.string,
  sorted: PropTypes.bool,
  columnStorageName: PropTypes.string,
  checkboxSize: PropTypes.string,
  sectionWidth: PropTypes.number,
  isHeaderVisible: PropTypes.bool,
  checkboxOptions: PropTypes.any.isRequired,
  isChecked: PropTypes.bool,
  onChange: PropTypes.func,
  isIndeterminate: PropTypes.bool,
  headerMenu: PropTypes.arrayOf(PropTypes.object),
  onClick: PropTypes.func,
  hasAccess: PropTypes.bool,
  resetColumnsSize: PropTypes.bool,
};

export default TableHeader;
