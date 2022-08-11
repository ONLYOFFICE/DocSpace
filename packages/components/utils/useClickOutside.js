import React, { useEffect } from "react";

export const useClickOutside = (ref, handler, ...deps) => {
  useEffect(() => {
    const handleClickOutside = (e) => {
      e.stopPropagation();
      if (ref.current && !ref.current.contains(e.target)) handler();
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref, ...deps]);
};
