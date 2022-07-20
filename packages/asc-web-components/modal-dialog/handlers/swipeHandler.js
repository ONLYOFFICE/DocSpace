import { isSmallTablet, isTouchDevice } from "../../utils/device";

let y1 = null;

export const handleTouchStart = (e) => {
  if (!(isTouchDevice && isSmallTablet())) {
    y1 = null;
    return false;
  }

  const firstTouch = e.touches[0];
  if (firstTouch.target.id !== "modal-header-swipe") {
    y1 = null;
    return false;
  }

  y1 = firstTouch.clientY;
};

export const handleTouchMove = (e, onClose) => {
  if (!y1) return 0;

  let y2 = e.touches[0].clientY;
  if (y2 - y1 > 120) onClose();

  return y1 - y2;
};
