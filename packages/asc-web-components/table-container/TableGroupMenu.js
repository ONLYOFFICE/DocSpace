import React, { useEffect } from "react";
import PropTypes from "prop-types";
import Checkbox from "../checkbox";
import { StyledTableGroupMenu } from "./StyledTableContainer";
import Button from "../button";
import ComboBox from "../combobox";

const TableGroupMenu = (props) => {
  const {
    isChecked,
    isIndeterminate,
    headerMenu,
    containerRef,
    onChange,
    checkboxOptions,
    columnStorageName,
    checkboxMargin,
    ...rest
  } = props;

  const onCheckboxChange = (e) => {
    onChange && onChange(e.target && e.target.checked);
  };

  const width = containerRef.current
    ? containerRef.current.clientWidth + "px"
    : "100%";

  useEffect(() => {
    const storageSize = localStorage.getItem(columnStorageName);
    if (containerRef.current)
      containerRef.current.style.gridTemplateColumns = storageSize;
  }, [containerRef]);

  return (
    <>
      <StyledTableGroupMenu
        width={width}
        className="table-container_group-menu"
        checkboxMargin={checkboxMargin}
        {...rest}
      >
        <Checkbox
          className="table-container_group-menu-checkbox"
          onChange={onCheckboxChange}
          isChecked={isChecked}
          isIndeterminate={isIndeterminate}
        />
        <ComboBox
          advancedOptions={checkboxOptions}
          className="table-container_group-menu-combobox not-selectable"
          options={[]}
          selectedOption={{}}
        />
        <div className="table-container_group-menu-separator" />
        {headerMenu.map((item, index) => {
          const { label, disabled, onClick } = item;
          return (
            <Button
              key={index}
              className="table-container_group-menu_button not-selectable"
              isDisabled={disabled}
              onClick={onClick}
              label={label}
            />
          );
        })}
      </StyledTableGroupMenu>
    </>
  );
};

TableGroupMenu.propTypes = {
  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  headerMenu: PropTypes.arrayOf(PropTypes.object),
  checkboxOptions: PropTypes.any.isRequired,
  onClick: PropTypes.func,
  onChange: PropTypes.func,
  containerRef: PropTypes.shape({ current: PropTypes.any }),
  columnStorageName: PropTypes.string,
  checkboxMargin: PropTypes.string,
};

export default TableGroupMenu;
