import { useState, useEffect } from "react";
import { size } from "@docspace/components/utils/device";

export const useIsMobileView = () => {
  const [isMobileView, setIsMobileView] = useState(false);

  const onCheckView = () => {
    window.innerWidth <= size.smallTablet
      ? setIsMobileView(true)
      : setIsMobileView(false);
  };

  useEffect(() => {
    onCheckView();

    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  return isMobileView;
};
