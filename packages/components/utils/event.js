export const handleAnyClick = (subscribe, handler) => {
  if (subscribe) {
    document.addEventListener("click", handler);
  } else {
    document.removeEventListener("click", handler);
  }
};
