import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import IconButton from "../icon-button";
import DropDown from "../drop-down";
import { StyledTableSettings } from "./StyledTableContainer";
import Checkbox from "../checkbox";

const TableSettings = ({ columns }) => {
  const [isOpen, setIsOpen] = useState(false);

  const ref = useRef();

  const onClick = () => {
    setIsOpen(!isOpen);
  };

  const clickOutsideAction = (e) => {
    const path = e.path || (e.composedPath && e.composedPath());
    const dropDownItem = path ? path.find((x) => x === ref.current) : null;
    if (dropDownItem) return;

    setIsOpen(false);
  };

  return (
    <StyledTableSettings
      className="table-container_header-settings-icon"
      ref={ref}
    >
      <IconButton
        color="#A3A9AE"
        hoverColor="#657077"
        size={12}
        isFill
        iconName="/static/images/settings.react.svg"
        onClick={onClick}
      />
      <DropDown
        className="table-container_settings"
        directionX="right"
        open={isOpen}
        clickOutsideAction={clickOutsideAction}
        withBackdrop={false}
      >
        {columns.map((column) => {
          const onChange = (e) =>
            column.onChange && column.onChange(column.key, e);

          return (
            column.onChange && (
              <Checkbox
                className="table-container_settings-checkbox"
                isChecked={column.enable}
                onChange={onChange}
                key={column.key}
                label={column.title}
              />
            )
          );
        })}
      </DropDown>
    </StyledTableSettings>
  );
};

TableSettings.propTypes = {
  columns: PropTypes.array.isRequired,
};

export default TableSettings;
