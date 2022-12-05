import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import toastr from "@docspace/components/toast/toastr";

import { ThemeKeys } from "@docspace/common/constants";

import { smallTablet } from "@docspace/components/utils/device";
import { showLoader, hideLoader } from "@docspace/common/utils";

import ThemePreview from "./theme-preview";

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
    color: ${(props) => props.theme.profile.themePreview.descriptionColor};
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
  const [currentTheme, setCurrentTheme] = useState(theme);

  const themeChange = async (theme) => {
    showLoader();

    try {
      setCurrentTheme(theme);
      await changeTheme(theme);
    } catch (error) {
      console.error(error);
      toastr.error(error);
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

  const isSystemTheme = currentTheme === ThemeKeys.SystemStr;

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
        <Text as="div" className="system-theme-description">
          {t("SystemThemeDescription")}
        </Text>
      </div>
      <div className="themes-container">
        <ThemePreview
          label={t("LightTheme")}
          isDisabled={isSystemTheme}
          theme="Light"
          accentColor={currentColorScheme.main.accent}
          themeId={selectedThemeId}
          value={ThemeKeys.BaseStr}
          isChecked={currentTheme === ThemeKeys.BaseStr}
          onChangeTheme={onChangeTheme}
        />
        <ThemePreview
          label={t("DarkTheme")}
          isDisabled={isSystemTheme}
          theme="Dark"
          accentColor={currentColorScheme.main.accent}
          themeId={selectedThemeId}
          value={ThemeKeys.DarkStr}
          isChecked={currentTheme === ThemeKeys.DarkStr}
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
