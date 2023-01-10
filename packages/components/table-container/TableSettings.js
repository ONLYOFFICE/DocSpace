import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
// import IconButton from "../icon-button";
import DropDown from "../drop-down";
import {
  StyledTableSettings,
  StyledSettingsIcon,
} from "./StyledTableContainer";
import Checkbox from "../checkbox";

const TableSettings = ({ columns, infoPanelVisible }) => {
  const [isOpen, setIsOpen] = useState(false);

  const ref = useRef();

  const onClick = () => {
    !infoPanelVisible && setIsOpen(!isOpen);
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
      <StyledSettingsIcon
        size={12}
        isFill
        iconName="/static/images/settings.desc.react.svg"
        onClick={onClick}
        isDisabled={infoPanelVisible}
      />
      <DropDown
        className="table-container_settings"
        directionX="right"
        open={isOpen}
        clickOutsideAction={clickOutsideAction}
        forwardedRef={ref}
        withBackdrop={false}
      >
        {columns.map((column) => {
          if (column.isDisabled) return;

          const onChange = (e) =>
            column.onChange && column.onChange(column.key, e);

          return (
            column.onChange && (
              <Checkbox
                className="table-container_settings-checkbox not-selectable"
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
