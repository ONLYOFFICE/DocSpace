import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

const Column = ({
  id,
  index,
  title,
  resizable,
  children,
  containerRef,
  className,
  ...rest
}) => {
  const [columnIndex, setColumnIndex] = useState(null);

  const getSubstring = (str) => str.substring(0, str.length - 1);

  const onMouseMove = (e) => {
    if (!columnIndex) return;
    const column = document.getElementById("column_" + columnIndex);

    const columnSize = column.getBoundingClientRect();
    const newWidth = e.clientX - columnSize.left;
    const percentWidth = (newWidth / containerRef.current.clientWidth) * 100;

    const { width, minWidth } = column.style;

    const clearPercent = getSubstring(width);

    if (percentWidth - 2 <= getSubstring(minWidth)) {
      return;
    }

    const offset = +percentWidth.toFixed(3) - clearPercent;
    const column2 = document.getElementById("column_" + (+columnIndex + 1));
    const column2Width = column2 && getSubstring(column2.style.width);

    if (column2) {
      if (+percentWidth.toFixed(3) < clearPercent) {
        const width2 = column2Width - offset;
        column2.style.width = width2 + "%";
      } else if (+percentWidth.toFixed(3) > clearPercent) {
        const width2 = +column2Width - offset;

        if (width2 - 2 <= getSubstring(column2.style.minWidth)) {
          return;
        }

        column2.style.width = width2 + "%";
      } else return;
    }

    column.style.width = percentWidth + "%";
  };

  const onMouseUp = () => {
    window.removeEventListener("mousemove", onMouseMove);
    window.removeEventListener("mouseup", onMouseUp);
  };

  const onMouseDown = (event) => {
    setColumnIndex(event.target.dataset.column);
  };

  useEffect(() => {
    if (columnIndex !== null) {
      window.addEventListener("mousemove", onMouseMove);
      window.addEventListener("mouseup", onMouseUp);
    }

    return () => {
      window.removeEventListener("mousemove", onMouseMove);
      window.removeEventListener("mouseup", onMouseUp);
    };
  }, [columnIndex, onMouseMove, onMouseUp]);

  return (
    <div
      style={{ minWidth: "10%", width: "20%" }}
      id={id}
      className={`${className} table-column`}
      {...rest}
    >
      <div className="header-container">
        {title}
        {index}
      </div>
      <div className="content-container">{children}</div>
      {resizable && (
        <div
          data-column={`${index}`}
          className="resize-handle not-selectable"
          onMouseDown={onMouseDown}
        />
      )}
    </div>
  );
};

Column.propTypes = {
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  className: PropTypes.string,
  style: PropTypes.object,
  children: PropTypes.any,
  index: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  title: PropTypes.string,
  resizable: PropTypes.bool,
  containerRef: PropTypes.shape({ current: PropTypes.any }),
};

export default Column;
