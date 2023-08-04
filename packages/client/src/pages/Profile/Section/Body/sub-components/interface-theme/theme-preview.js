import React from "react";
import RadioButton from "@docspace/components/radio-button";

import { StyledWrapper } from "./styled-preview";
import Preview from "SRC_DIR/pages/PortalSettings/categories/common/Appearance/preview";

const ThemePreview = (props) => {
  const {
    label,
    isDisabled,
    isChecked,
    onChangeTheme,
    value,
    theme,
    accentColor,
    themeId,
  } = props;

  return (
    <StyledWrapper>
      <div className="card-header">
        <RadioButton
          classNameInput={`theme-${theme.toLowerCase()}`}
          name={`theme-option-${value}`}
          label={label}
          onClick={onChangeTheme}
          value={value}
          isDisabled={isDisabled}
          isChecked={isChecked}
        />
      </div>
      <Preview
        appliedColorAccent={accentColor}
        floatingButtonClass="floating-btn"
        selectThemeId={themeId}
        previewAccent={accentColor}
        themePreview={theme}
        withBorder={false}
        withTileActions={false}
      />
    </StyledWrapper>
  );
};

export default ThemePreview;
