import { makeAutoObservable } from "mobx";
import { getAppearanceTheme } from "@docspace/common/api/settings";
import { Base } from "@docspace/components/themes";

class LoginStore {
  currentColorScheme: ITheme | null = null;
  appearanceTheme: IThemes | [] = [];
  selectedThemeId: number | null = null;
  theme = Base;

  constructor(currentColorScheme: ITheme) {
    makeAutoObservable(this);
    this.currentColorScheme = currentColorScheme;
  }

  setTheme = (theme) => {
    this.theme = theme;
  };

  setCurrentColorScheme = (currentColorScheme: ITheme) => {
    this.currentColorScheme = currentColorScheme;
  };

  setAppearanceTheme = (theme: IThemes) => {
    this.appearanceTheme = theme;
  };

  setSelectThemeId = (selected: number) => {
    this.selectedThemeId = selected;
  };

  getAppearanceTheme = async () => {
    const res: IThemes = await getAppearanceTheme();

    const currentColorScheme = res.themes.find((theme: ITheme) => {
      return res.selected === theme.id;
    });

    this.setAppearanceTheme(res.themes);
    this.setSelectThemeId(res.selected);
    if (currentColorScheme) this.setCurrentColorScheme(currentColorScheme);
  };
}

function initLoginStore(currentColorScheme: ITheme) {
  const loginStore = new LoginStore(currentColorScheme);

  return {
    loginStore,
  };
}

export default initLoginStore;
