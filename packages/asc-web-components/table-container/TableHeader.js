import React from "react";
import PropTypes from "prop-types";
import throttle from "lodash.throttle";
import Text from "../text";
import globalColors from "../utils/globalColors";
import {
  StyledSettingsIcon,
  StyledTableHeader,
  StyledTableRow,
} from "./StyledTableContainer";
import Checkbox from "../checkbox";

const TABLE_SIZE = "tableSize";

class TableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.state = { columnIndex: null };

    this.headerRef = React.createRef();
    this.throttledResize = throttle(this.onResize, 0);
  }

  componentDidMount() {
    //this.onResize();
    window.addEventListener("resize", this.throttledResize);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
  }

  getSubstring = (str) => str.substring(0, str.length - 2);

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
    if (newWidth <= 150) {
      widths[+columnIndex] = widths[+columnIndex];
    } else {
      const offset = +this.getSubstring(widths[+columnIndex]) - newWidth;
      const column2Width = +this.getSubstring(widths[+columnIndex + 1]);

      //getSubstring(widths[+columnIndex])
      if (column2Width + offset >= 150) {
        widths[+columnIndex] = newWidth + "px";
        widths[+columnIndex + 1] = column2Width + offset + "px";
      }
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

    const storageSize = localStorage.getItem(TABLE_SIZE);
    const tableContainer = storageSize
      ? storageSize.split(" ")
      : containerRef.current.style.gridTemplateColumns.split(" ");

    const containerWidth = +containerRef.current.clientWidth;
    const newContainerWidth = containerWidth - 32 - 80 - 24;

    let str = "";

    if (tableContainer.length > 1) {
      const gridTemplateColumns = [];

      const oldWidth = tableContainer
        .map((column) => +this.getSubstring(column))
        .reduce((x, y) => x + y);

      for (let index in tableContainer) {
        const item = tableContainer[index];

        if (item !== "24px" && item !== "32px" && item !== "80px") {
          const percent = (+this.getSubstring(item) / oldWidth) * 100;
          const newItemWidth = (containerWidth * percent) / 100 + "px";

          gridTemplateColumns.push(newItemWidth);
        } else {
          gridTemplateColumns.push(item);
        }

        str = gridTemplateColumns.join(" ");
      }
    } else {
      const column = (newContainerWidth * 40) / 100 + "px";
      const otherColumns = (newContainerWidth * 20) / 100 + "px";

      str = `32px ${column} ${otherColumns} ${otherColumns} ${otherColumns} 80px 24px`;
    }
    containerRef.current.style.gridTemplateColumns = str;
    this.headerRef.current.style.gridTemplateColumns = str;

    localStorage.setItem(TABLE_SIZE, str);
  };

  onChange = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  render() {
    const { columns, ...rest } = this.props;

    return (
      <StyledTableHeader
        className="table-container_header"
        ref={this.headerRef}
        {...rest}
      >
        <StyledTableRow>
          <Checkbox onChange={this.onChange} checked={false} />

          {columns.map((column, index) => {
            return (
              <div
                className="table-container_header-cell"
                id={`column_${index + 1}`}
                key={column.key}
              >
                <div style={{ display: "flex", userSelect: "none" }}>
                  <Text
                    fontWeight={600}
                    color={globalColors.gray}
                    className="header-container"
                  >
                    {column.title}
                  </Text>
                  {column.resizable && (
                    <div
                      data-column={`${index + 1}`}
                      className="resize-handle not-selectable"
                      onMouseDown={this.onMouseDown}
                    />
                  )}
                </div>
              </div>
            );
          })}

          <div className="table-container_header-cell">
            <StyledSettingsIcon />
          </div>
        </StyledTableRow>
      </StyledTableHeader>
    );
  }
}

TableHeader.propTypes = {
  containerRef: PropTypes.shape({ current: PropTypes.any }),
  columns: PropTypes.array,
  setSelected: PropTypes.func,
};

export default TableHeader;
