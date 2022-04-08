let y1 = null;

export const handleTouchStart = (e) => {
  const firstTouch = e.touches[0];

  if (firstTouch.target.id !== "modal-header-swipe") {
    y1 = null;
    return false;
  }

  y1 = firstTouch.clientY;
};

export const handleTouchMove = (e, onClose) => {
  if (!y1) return false;

  let y2 = e.touches[0].clientY;
  if (y2 - y1 > 120) onClose();

  return y1 - y2;
};
