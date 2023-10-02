import { useEffect, useRef } from "react";
import { useLocation } from "react-router-dom";

export default function ScrollToTop() {
  const { pathname, state } = useLocation();
  const scrollRef = useRef();

  useEffect(() => {
    scrollRef.current = document.querySelector(
      "#customScrollBar > .scroll-wrapper > .scroller"
    );
  }, []);

  useEffect(() => {
    !state?.disableScrollToTop &&
      scrollRef.current &&
      scrollRef.current.scrollTo(0, 0);
  }, [pathname]);

  return null;
}
