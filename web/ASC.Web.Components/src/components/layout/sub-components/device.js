const size = {
  mobile: "375px",
  tablet: "768px",
  desktop: "1024px"
};

const device = {
  mobile: `(max-width: ${size.mobile})`,
  tablet: `(max-width: ${size.tablet})`,
  desktop: `(max-width: ${size.desktop})`
};

export default device;