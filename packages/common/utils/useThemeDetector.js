import { useEffect, useState } from "react";
import { getSystemTheme } from "../utils";

export const useThemeDetector = () => {
  const isDesktopClient = window["AscDesktopEditor"] !== undefined;
  const [systemTheme, setSystemTheme] = useState(getSystemTheme());

  const systemThemeListener = (e) => {
    setSystemTheme(e.matches ? "Dark" : "Base");
  };

  useEffect(() => {
    if (isDesktopClient) return;

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    mediaQuery.addEventListener("change", systemThemeListener);

    return () => {
      if (isDesktopClient) return;

      mediaQuery.removeEventListener("change", systemThemeListener);
    };
  }, []);

  return systemTheme;
};
