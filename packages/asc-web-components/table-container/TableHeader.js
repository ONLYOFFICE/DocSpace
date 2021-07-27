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

const TABLE_SIZE = "tableSize";
const minColumnSize = 80;

class TableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.state = { columnIndex: null };

    this.headerRef = React.createRef();
    this.throttledResize = throttle(this.onResize, 0);
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

    if (leftColumn.clientWidth <= minColumnSize) {
      if (colIndex === 1) return false;
      return this.moveToLeft(widths, newWidth, colIndex - 1);
    }

    const offset = this.getSubstring(widths[+columnIndex]) - newWidth;
    const column2Width = this.getSubstring(widths[colIndex]);

    const leftColumnWidth = column2Width - offset;
    const newLeftWidth =
      leftColumnWidth < minColumnSize ? minColumnSize : leftColumnWidth;

    widths[colIndex] = newLeftWidth + "px";
    widths[+columnIndex] =
      this.getSubstring(widths[+columnIndex]) + offset + "px";
  };

  moveToRight = (widths, newWidth, index) => {
    const { columnIndex } = this.state;

    let rightColumn;
    let colIndex = index ? index : +columnIndex + 1;

    while (colIndex !== this.props.columns.length - 1) {
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
      if (colIndex === this.props.columns.length - 1) return false;
      return this.moveToRight(widths, newWidth, colIndex + 1);
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

    //getSubstring(widths[+columnIndex])
    if (newWidth <= minColumnSize) {
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
      TABLE_SIZE,
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
    const { containerRef } = this.props;

    const container = containerRef.current
      ? containerRef.current
      : document.getElementById("table-container");

    const storageSize = localStorage.getItem(TABLE_SIZE);
    const tableContainer = storageSize
      ? storageSize.split(" ")
      : container.style.gridTemplateColumns.split(" ");

    const containerWidth = +container.clientWidth;
    const newContainerWidth = containerWidth - 32 - 80 - 24;

    const enableColumns = this.props.columns
      .filter((x) => !x.default)
      .filter((x) => x.enable);

    const isSingleTable = enableColumns.length > 0;

    let str = "";
    let disableColumnWidth = 0;

    if (tableContainer.length > 1) {
      const gridTemplateColumns = [];

      const oldWidth = tableContainer
        .map((column) => this.getSubstring(column))
        .reduce((x, y) => x + y);

      for (let index in tableContainer) {
        const item = tableContainer[index];

        //TODO: need refactoring this code
        const column = document.getElementById("column_" + index);
        const enable =
          index == 0 ||
          index == tableContainer.length - 1 ||
          (column && column.dataset.enable === "true");

        const isActiveNow = item === "0px" && enable;

        if (!enable) {
          gridTemplateColumns.push("0px");
          gridTemplateColumns[1] =
            this.getSubstring(gridTemplateColumns[1]) +
            this.getSubstring(item) +
            "px";
        } else if (item !== "24px" && item !== "32px" && item !== "80px") {
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

            if (isActiveNow) {
              //add logic to new columns widths
              gridTemplateColumns[1] =
                this.getSubstring(gridTemplateColumns[1]) -
                this.getSubstring(newItemWidth) +
                "px";
            }

            gridTemplateColumns.push(newItemWidth);
          }
          //TODO: need refactoring this code
        } else {
          gridTemplateColumns.push(item);
        }

        str = gridTemplateColumns.join(" ");
      }
    } else {
      const column =
        (newContainerWidth * (isSingleTable ? 60 : 100)) / 100 + "px";
      const percent = 40 / enableColumns.length;
      const otherColumns = (newContainerWidth * percent) / 100 + "px";

      str = `32px ${column} `;
      for (let col of this.props.columns) {
        if (!col.default) {
          str += col.enable ? `${otherColumns} ` : "0px ";
        }
      }

      str += "80px 24px";
    }
    container.style.gridTemplateColumns = str;
    this.headerRef.current.style.gridTemplateColumns = str;
    this.headerRef.current.style.width = containerWidth + "px";

    localStorage.setItem(TABLE_SIZE, str);
  };

  onChange = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  render() {
    const { columns, sortBy, ...rest } = this.props;

    //console.log("TABLE HEADER RENDER", columns);

    return (
      <>
        <StyledTableHeader
          className="table-container_header"
          ref={this.headerRef}
          {...rest}
        >
          <StyledTableRow>
            <Checkbox onChange={this.onChange} isChecked={false} />

            {columns.map((column, index) => {
              const nextColumn = this.getNextColumn(columns, index);
              const resizable = nextColumn ? nextColumn.resizable : false;

              return (
                <TableHeaderCell
                  key={column.key}
                  index={index}
                  column={column}
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
        <StyledEmptyTableContainer />
      </>
    );
  }
}

TableHeader.propTypes = {
  containerRef: PropTypes.shape({ current: PropTypes.any }),
  columns: PropTypes.array.isRequired,
  setSelected: PropTypes.func,
  sortBy: PropTypes.string,
};

export default TableHeader;
