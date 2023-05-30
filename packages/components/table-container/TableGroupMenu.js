import React from "react";
import PropTypes from "prop-types";
import Checkbox from "../checkbox";
import {
  StyledTableGroupMenu,
  StyledScrollbar,
  StyledInfoPanelToggleColorThemeWrapper,
} from "./StyledTableContainer";
import ComboBox from "../combobox";
import GroupMenuItem from "./GroupMenuItem";
import { useTranslation } from "react-i18next";
import IconButton from "../icon-button";
import TriangleNavigationDownReactSvgUrl from "PUBLIC_DIR/images/triangle.navigation.down.react.svg?url";
import PanelReactSvgUrl from "PUBLIC_DIR/images/panel.react.svg?url";
import { ThemeType } from "@docspace/components/ColorTheme";

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
    isMobileView,
    isBlocked,
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
          id="menu-checkbox_selected-all-file"
          className="table-container_group-menu-checkbox"
          onChange={onCheckboxChange}
          isChecked={isChecked}
          isIndeterminate={isIndeterminate}
          title={t("Common:MainHeaderSelectAll")}
        />
        <ComboBox
          id="menu-combobox"
          comboIcon={TriangleNavigationDownReactSvgUrl}
          noBorder
          advancedOptions={checkboxOptions}
          className="table-container_group-menu-combobox not-selectable"
          options={[]}
          selectedOption={{}}
          manualY="42px"
          manualX="-32px"
          title={t("Common:TitleSelectFile")}
          isMobileView={isMobileView}
        />
        <div className="table-container_group-menu-separator" />
        <StyledScrollbar>
          {headerMenu.map((item, index) => (
            <GroupMenuItem key={index} item={item} isBlocked={isBlocked} />
          ))}
        </StyledScrollbar>
        {!withoutInfoPanelToggler && (
          <StyledInfoPanelToggleColorThemeWrapper
            themeId={ThemeType.InfoPanelToggle}
            isInfoPanelVisible={isInfoPanelVisible}
          >
            <div className="info-panel-toggle-bg">
              <IconButton
                id="info-panel-toggle--open"
                className="info-panel-toggle"
                iconName={PanelReactSvgUrl}
                size="16"
                isFill={true}
                onClick={toggleInfoPanel}
              />
            </div>
          </StyledInfoPanelToggleColorThemeWrapper>
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
