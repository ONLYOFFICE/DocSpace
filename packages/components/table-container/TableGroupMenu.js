import React from "react";
import PropTypes from "prop-types";
import Checkbox from "../checkbox";
import {
  StyledTableGroupMenu,
  StyledScrollbar,
  StyledInfoPanelToggleWrapper,
} from "./StyledTableContainer";
import ComboBox from "../combobox";
import GroupMenuItem from "./GroupMenuItem";
import { useTranslation } from "react-i18next";
import IconButton from "../icon-button";

const TableGroupMenu = (props) => {
  const {
    isChecked,
    isIndeterminate,
    headerMenu,
    onChange,
    checkboxOptions,
    checkboxMargin,
    isInfoPanelVisible,
    toggleInfoPanel,
    withoutInfoPanelToggler,
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
        {!withoutInfoPanelToggler && (
          <StyledInfoPanelToggleWrapper isInfoPanelVisible={isInfoPanelVisible}>
            <div className="info-panel-toggle-bg">
              <IconButton
                className="info-panel-toggle"
                iconName="images/panel.react.svg"
                size="16"
                isFill={true}
                onClick={toggleInfoPanel}
              />
            </div>
          </StyledInfoPanelToggleWrapper>
        )}
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
  withoutInfoPanelToggler: PropTypes.bool,
};
export default TableGroupMenu;
