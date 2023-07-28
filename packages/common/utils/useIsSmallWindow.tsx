import { useState, useEffect } from "react";
import { isDesktop } from "react-device-detect";

export const useIsSmallWindow = (windowWidth: number): boolean => {
  const [isSmallWindow, setIsSmallWindow] = useState(false);

  const onCheckView = () => {
    if (isDesktop && window.innerWidth < windowWidth) {
      setIsSmallWindow(true);
    } else {
      setIsSmallWindow(false);
    }
  };

  useEffect(() => {
    onCheckView();

    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  return isSmallWindow;
};
