import { useEffect, useState } from "react";

const SystemThemeDetector = ({ userTheme, setTheme, isBase }) => {
  const useSystemTheme = () => {
    const [systemTheme, setSystemTheme] = useState(
      window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "Dark"
        : "Base"
    );

    const systemThemeListener = (e) => {
      setSystemTheme(e.matches ? "Dark" : "Base");
    };

    useEffect(() => {
      const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
      mediaQuery.addListener(systemThemeListener);
      return () => mediaQuery.removeListener(systemThemeListener);
    }, []);

    return systemTheme;
  };

  const currentTheme = isBase ? "Base" : "Dark";
  const systemTheme = useSystemTheme();

  useEffect(() => {
    if (userTheme === "System" && currentTheme !== systemTheme)
      setTheme(systemTheme);
  }, [systemTheme]);

  return null;
};

export default SystemThemeDetector;
