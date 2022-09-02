import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import { ThemeKeys } from "@docspace/common/constants";

import ThemePreview from "./theme-preview";

import { Base, Dark } from "@docspace/components/themes";

import { smallTablet } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .system-theme-description {
    padding: 4px 0 4px 30px;
    max-width: 295px;
  }

  .themes-container {
    display: flex;
    flex-wrap: wrap;
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
  const { theme, changeTheme, setIsLoading } = props;

  const themeChange = async (theme) => {
    try {
      setIsLoading(true);
      await changeTheme(theme);
    } finally {
      setIsLoading(false);
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
          t={t}
          label={t("LightTheme")}
          isDisabled={isSystemTheme}
          theme={Base}
          value={ThemeKeys.BaseStr}
          isChecked={theme === ThemeKeys.BaseStr}
          onChangeTheme={onChangeTheme}
        />
        <ThemePreview
          t={t}
          label={t("DarkTheme")}
          isDisabled={isSystemTheme}
          theme={Dark}
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

export default inject(({ auth, peopleStore }) => {
  const { userStore } = auth;
  const { loadingStore } = peopleStore;

  const { changeTheme, user } = userStore;

  return {
    changeTheme,
    theme: user.theme,
    setIsLoading: loadingStore.setIsLoading,
  };
})(observer(InterfaceTheme));
