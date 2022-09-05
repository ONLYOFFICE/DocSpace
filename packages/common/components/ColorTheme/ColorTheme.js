import React, { forwardRef } from "react";
import { inject, observer } from "mobx-react";

import {
  ButtonTheme,
  MainButtonTheme,
  CatalogItemTheme,
  BadgeTheme,
  SubmenuTextTheme,
  SubmenuItemLabelTheme,
  ToggleButtonTheme,
  TabsContainerTheme,
  IconButtonTheme,
  IconButtonPinTheme,
  IndicatorFilterButtonTheme,
  FilterBlockItemTagTheme,
  IconWrapperTheme,
  CalendarTheme,
  VersionBadgeTheme,
  TextareaTheme,
  InputBlockTheme,
  TextInputTheme,
  ComboButtonTheme,
  LinkForgotPasswordTheme,
  LoadingButtonTheme,
  FloatingButtonTheme,
  InfoPanelToggleTheme,
  LinkTheme,
  SliderTheme,
  IndicatorLoaderTheme,
} from "./styled";
import { ThemeType } from "./constants";

const ColorTheme = forwardRef(
  ({ currentColorScheme, isVersion, themeId, ...props }, ref) => {
    switch (themeId) {
      case ThemeType.Button: {
        return (
          <ButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.MainButton: {
        return (
          <MainButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.CatalogItem: {
        return (
          <CatalogItemTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.Badge: {
        return (
          <BadgeTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.SubmenuText: {
        return (
          <SubmenuTextTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.SubmenuItemLabel: {
        return (
          <SubmenuItemLabelTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.ToggleButton: {
        return (
          <ToggleButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.TabsContainer: {
        return (
          <TabsContainerTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.IconButton: {
        return (
          <IconButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.IconButtonPin: {
        return (
          <IconButtonPinTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.IndicatorFilterButton: {
        return (
          <IndicatorFilterButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.FilterBlockItemTag: {
        return (
          <FilterBlockItemTagTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.IconWrapper: {
        return (
          <IconWrapperTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.Calendar: {
        return (
          <CalendarTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.VersionBadge: {
        return (
          <VersionBadgeTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            $isVersion={isVersion}
            ref={ref}
          />
        );
      }
      case ThemeType.Textarea: {
        return (
          <TextareaTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.InputBlock: {
        return (
          <InputBlockTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.TextInput: {
        return (
          <TextInputTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.ComboButton: {
        return (
          <ComboButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.LinkForgotPassword: {
        return (
          <LinkForgotPasswordTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.LoadingButton: {
        return (
          <LoadingButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.FloatingButton: {
        return (
          <FloatingButtonTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.InfoPanelToggle: {
        return (
          <InfoPanelToggleTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.Link: {
        return (
          <LinkTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.Slider: {
        return (
          <SliderTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
      case ThemeType.IndicatorLoader: {
        return (
          <IndicatorLoaderTheme
            {...props}
            $currentColorScheme={currentColorScheme}
            ref={ref}
          />
        );
      }
    }
  }
);

export default inject(({ auth, loginStore }) => {
  if (loginStore) {
    const { currentColorScheme } = loginStore;
    return { currentColorScheme };
  } else {
    const { settingsStore } = auth;
    const { currentColorScheme } = settingsStore;
    return {
      currentColorScheme,
    };
  }
})(observer(ColorTheme));
