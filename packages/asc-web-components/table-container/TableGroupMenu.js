import React from "react";
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
    onChange,
    checkboxOptions,
    checkboxMargin,
    ...rest
  } = props;

  const onCheckboxChange = (e) => {
    onChange && onChange(e.target && e.target.checked);
  };

  return (
    <>
      <StyledTableGroupMenu
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
          comboIcon="/static/images/triangle.navigation.down.react.svg"
          noBorder
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
  headerMenu: PropTypes.arrayOf(PropTypes.object).isRequired,
  checkboxOptions: PropTypes.any.isRequired,
  onClick: PropTypes.func,
  onChange: PropTypes.func,
  checkboxMargin: PropTypes.string,
};

export default TableGroupMenu;
