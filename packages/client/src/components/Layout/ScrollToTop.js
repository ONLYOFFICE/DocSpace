import { useEffect, useRef } from "react";
import { useLocation } from "react-router-dom";

export default function ScrollToTop() {
  const { pathname } = useLocation();
  const scrollRef = useRef();

  useEffect(() => {
    scrollRef.current = document.querySelector(
      "#customScrollBar > .scroll-body"
    );
  }, []);

  useEffect(() => {
    scrollRef.current && scrollRef.current.scrollTo(0, 0);
  }, [pathname]);

  return null;
}
