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
} from "./styled";
import { ThemeType } from "./constants";

const ColorTheme = forwardRef((props, ref) => {
  switch (props.type) {
    case ThemeType.Button: {
      return <ButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.MainButton: {
      return <MainButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.CatalogItem: {
      return <CatalogItemTheme ref={ref} {...props} />;
    }
    case ThemeType.Badge: {
      return <BadgeTheme ref={ref} {...props} />;
    }
    case ThemeType.SubmenuText: {
      return <SubmenuTextTheme ref={ref} {...props} />;
    }
    case ThemeType.SubmenuItemLabel: {
      return <SubmenuItemLabelTheme ref={ref} {...props} />;
    }
    case ThemeType.ToggleButton: {
      return <ToggleButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.TabsContainer: {
      return <TabsContainerTheme ref={ref} {...props} />;
    }
    case ThemeType.IconButton: {
      return <IconButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.IconButtonPin: {
      return <IconButtonPinTheme ref={ref} {...props} />;
    }
    case ThemeType.IndicatorFilterButton: {
      return <IndicatorFilterButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.FilterBlockItemTag: {
      return <FilterBlockItemTagTheme ref={ref} {...props} />;
    }
    case ThemeType.IconWrapper: {
      return <IconWrapperTheme ref={ref} {...props} />;
    }
    case ThemeType.Calendar: {
      return <CalendarTheme ref={ref} {...props} />;
    }
    case ThemeType.VersionBadge: {
      return <VersionBadgeTheme ref={ref} {...props} />;
    }
    case ThemeType.Textarea: {
      return <TextareaTheme ref={ref} {...props} />;
    }
    case ThemeType.InputBlock: {
      return <InputBlockTheme ref={ref} {...props} />;
    }
    case ThemeType.TextInput: {
      return <TextInputTheme ref={ref} {...props} />;
    }
    case ThemeType.ComboButton: {
      return <ComboButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.LinkForgotPassword: {
      return <LinkForgotPasswordTheme ref={ref} {...props} />;
    }
    case ThemeType.LoadingButton: {
      return <LoadingButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.FloatingButton: {
      return <FloatingButtonTheme ref={ref} {...props} />;
    }
    case ThemeType.InfoPanelToggle: {
      return <InfoPanelToggleTheme ref={ref} {...props} />;
    }
  }
});

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { currentColorScheme } = settingsStore;

  return {
    currentColorScheme,
  };
})(observer(ColorTheme));
