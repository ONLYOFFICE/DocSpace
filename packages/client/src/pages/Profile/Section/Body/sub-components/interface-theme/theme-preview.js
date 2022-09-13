import React from "react";
import RadioButton from "@docspace/components/radio-button";

import { StyledWrapper } from "./styled-preview";
import Preview from "../../../../../PortalSettings/categories/common/settingsAppearance/preview";

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
          name={`theme-option-${value}`}
          label={label}
          onClick={onChangeTheme}
          value={value}
          isDisabled={isDisabled}
          isChecked={isChecked}
        />
      </div>
      <Preview
        selectThemeId={themeId}
        selectAccentColor={accentColor}
        themePreview={theme}
        withBorder={false}
        withTileActions={false}
      />
    </StyledWrapper>
  );
};

export default ThemePreview;
