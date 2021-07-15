import React from "react";
import PropTypes from "prop-types";
import Checkbox from "../checkbox";
import { StyledTableGroupMenu } from "./StyledTableContainer";
import Button from "../button";

const TableGroupMenu = (props) => {
  const { isChecked, isIndeterminate, headerMenu, onChange } = props;

  const onCheckboxChange = (e) => {
    onChange && onChange(e.target && e.target.checked);
  };

  return (
    <StyledTableGroupMenu className="table-container_group-menu">
      <Checkbox
        onChange={onCheckboxChange}
        isChecked={isChecked}
        isIndeterminate={isIndeterminate}
      />
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
  );
};

TableGroupMenu.propTypes = {
  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  headerMenu: PropTypes.arrayOf(PropTypes.object),
  onClick: PropTypes.func,
  onChange: PropTypes.func,
};

export default TableGroupMenu;
