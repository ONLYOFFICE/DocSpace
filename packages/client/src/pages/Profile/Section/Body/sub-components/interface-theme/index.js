import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import { ThemeKeys } from "@docspace/common/constants";

import ThemePreview from "./theme-preview";

import { smallTablet } from "@docspace/components/utils/device";
import { showLoader, hideLoader } from "@docspace/common/utils";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .checkbox {
    height: 20px;
    margin-right: 8px !important;
  }

  .system-theme-description {
    padding: 4px 0 4px 24px;
    max-width: 295px;
  }

  .themes-container {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
    gap: 20px;

    @media ${smallTablet} {
      display: none;
    }
  }

  .mobile-themes-container {
    display: none;

    @media ${smallTablet} {
      display: flex;
      padding-left: 30px;
    }
  }
`;

const InterfaceTheme = (props) => {
  const { t } = useTranslation(["Profile", "Common"]);
  const { theme, changeTheme, currentColorScheme, selectedThemeId } = props;

  const themeChange = async (theme) => {
    showLoader();

    try {
      await changeTheme(theme);
    } finally {
      hideLoader();
    }
  };

  const onChangeTheme = (e) => {
    const target = e.currentTarget;
    if (target.isChecked) return;
    themeChange(target.value);
  };

  const onChangeSystemTheme = (e) => {
    const isChecked = (e.currentTarget || e.target).checked;

    if (!isChecked) {
      themeChange(ThemeKeys.BaseStr);
    } else {
      themeChange(ThemeKeys.SystemStr);
    }
  };

  const isSystemTheme = theme === ThemeKeys.SystemStr;

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("InterfaceTheme")}
      </Text>

      <div>
        <Checkbox
          value={ThemeKeys.SystemStr}
          label={t("SystemTheme")}
          isChecked={isSystemTheme}
          onChange={onChangeSystemTheme}
        />
        <Text as="div" className="system-theme-description" color="#A3A9AE">
          {t("SystemThemeDescription")}
        </Text>
      </div>
      <div className="themes-container">
        <ThemePreview
          label={t("LightTheme")}
          isDisabled={isSystemTheme}
          theme="Light"
          accentColor={currentColorScheme.accentColor}
          themeId={selectedThemeId}
          value={ThemeKeys.BaseStr}
          isChecked={theme === ThemeKeys.BaseStr}
          onChangeTheme={onChangeTheme}
        />
        <ThemePreview
          label={t("DarkTheme")}
          isDisabled={isSystemTheme}
          theme="Dark"
          accentColor={currentColorScheme.accentColor}
          themeId={selectedThemeId}
          value={ThemeKeys.DarkStr}
          isChecked={theme === ThemeKeys.DarkStr}
          onChangeTheme={onChangeTheme}
        />
      </div>

      <div className="mobile-themes-container">
        <RadioButtonGroup
          orientation="vertical"
          name="interface-theme"
          options={[
            { value: ThemeKeys.BaseStr, label: t("LightTheme") },
            { value: ThemeKeys.DarkStr, label: t("DarkTheme") },
          ]}
          onClick={onChangeTheme}
          selected={theme}
          spacing="12px"
          isDisabled={isSystemTheme}
        />
      </div>
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { userStore, settingsStore } = auth;

  const { changeTheme, user } = userStore;
  const { currentColorScheme, selectedThemeId } = settingsStore;

  return {
    changeTheme,
    theme: user.theme,
    currentColorScheme,
    selectedThemeId,
  };
})(observer(InterfaceTheme));
