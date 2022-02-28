import React from "react";
import PropTypes from "prop-types";
import Checkbox from "../checkbox";
import { StyledTableGroupMenu, StyledScrollbar } from "./StyledTableContainer";
import ComboBox from "../combobox";
import GroupMenuItem from "./GroupMenuItem";
import { useTranslation } from "react-i18next";

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

  const { t } = useTranslation("Common");

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
          title={t("Common:MainHeaderSelectAll")}
        />
        <ComboBox
          comboIcon="/static/images/triangle.navigation.down.react.svg"
          noBorder
          advancedOptions={checkboxOptions}
          className="table-container_group-menu-combobox not-selectable"
          options={[]}
          selectedOption={{}}
          manualY="42px"
          manualX="-32px"
          title={t("Common:TitleSelectFile")}
        />
        <div className="table-container_group-menu-separator" />
        <StyledScrollbar>
          {headerMenu.map((item, index) => (
            <GroupMenuItem key={index} item={item} />
          ))}
        </StyledScrollbar>
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
