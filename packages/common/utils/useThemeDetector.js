import { useEffect, useState } from "react";

export const useThemeDetector = () => {
  const isDesktopClient = window["AscDesktopEditor"] !== undefined;
  const [systemTheme, setSystemTheme] = useState(
    isDesktopClient
      ? window?.RendererProcessVariable?.theme?.type === "dark"
        ? "Dark"
        : "Base"
      : window?.matchMedia("(prefers-color-scheme: dark)").matches
      ? "Dark"
      : "Base"
  );

  const systemThemeListener = (e) => {
    setSystemTheme(e.matches ? "Dark" : "Base");
  };

  useEffect(() => {
    if (isDesktopClient) return;

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    mediaQuery.addListener(systemThemeListener);

    return () => {
      if (isDesktopClient) return;

      mediaQuery.removeListener(systemThemeListener);
    };
  }, []);

  return systemTheme;
};
