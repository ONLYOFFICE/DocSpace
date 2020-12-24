import { useEffect } from "react";
import { useLocation } from "react-router-dom";

export default function ScrollToTop() {
  const { pathname } = useLocation();

  useEffect(() => {
    const documentScrollElement = document.getElementById("customScrollBar");
    documentScrollElement && documentScrollElement.scrollTo(0, 0);
  }, [pathname]);

  return null;
}
