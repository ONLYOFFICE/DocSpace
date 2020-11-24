export const size = {
  mobile: 375,
  smallTablet: 600,
  tablet: 1024,
  desktop: 1025,
};

export const mobile = `(max-width: ${size.mobile}px)`;

export const smallTablet = `(max-width: ${size.smallTablet}px)`;

export const tablet = `(max-width: ${size.tablet}px)`;

export const desktop = `(min-width: ${size.desktop}px)`;

export const isMobile = () => {
  return window.innerWidth < size.mobile;
};

export const isSmallTablet = () => {
  return window.innerWidth < size.smallTablet;
};

export const isTablet = () => {
  return window.innerWidth <= size.tablet && window.innerWidth >= size.mobile;
};

export const isDesktop = () => {
  return window.innerWidth >= size.desktop;
};
