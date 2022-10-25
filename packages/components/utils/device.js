import { checkIsSSR } from "@docspace/common/utils";

export const size = {
  mobile: 375,
  hugeMobile: 428,
  smallTablet: 600,
  tablet: 1024,
  desktop: 1025,
  hugeDesktop: 1440,
};

export const mobile = `(max-width: ${size.mobile}px)`;

export const hugeMobile = `(max-width: ${size.hugeMobile}px)`;

export const smallTablet = `(max-width: ${size.smallTablet}px)`;

export const tablet = `(max-width: ${size.tablet}px)`;

export const desktop = `(min-width: ${size.desktop}px)`;

export const hugeDesktop = `(max-width: ${size.hugeDesktop}px)`;

export const isMobile = () => {
  return window.innerWidth <= size.mobile;
};

export const isHugeMobile = () => {
  return window.innerWidth <= size.hugeMobile;
};

export const isSmallTablet = () => {
  return window.innerWidth < size.smallTablet;
};

export const isTablet = () => {
  return (
    window.innerWidth <= size.tablet && window.innerWidth >= size.hugeMobile
  );
};

export const isDesktop = () => {
  if (!checkIsSSR()) {
    return window.innerWidth >= size.desktop;
  } else return false;
};

export const isTouchDevice = !!(
  typeof window !== "undefined" &&
  typeof navigator !== "undefined" &&
  ("ontouchstart" in window || navigator.msMaxTouchPoints > 0)
);

export const getModalType = () => {
  return window.innerWidth < size.desktop ? "aside" : "modal";
};
