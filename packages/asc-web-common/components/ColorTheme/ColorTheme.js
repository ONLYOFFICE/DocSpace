import React from "react";
import { inject, observer } from "mobx-react";

import {
  ButtonTheme,
  MainButtonTheme,
  CatalogItemTheme,
  BadgeTheme,
} from "./styled";
import { ThemeType } from "./constants";

// TODO: default
const ColorTheme = (props) => {
  switch (props.type) {
    case ThemeType.Button: {
      return <ButtonTheme {...props} />;
    }
    case ThemeType.MainButton: {
      return <MainButtonTheme {...props} />;
    }
    case ThemeType.CatalogItem: {
      return <CatalogItemTheme {...props} />;
    }
    case ThemeType.Badge: {
      return <BadgeTheme {...props} />;
    }
  }
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { currentColorScheme } = settingsStore;

  return {
    currentColorScheme,
  };
})(observer(ColorTheme));
