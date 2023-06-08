import PropTypes from "prop-types";
import ThemeProvider from "../../theme-provider";

const ThemeWrapper = ({ theme, children }) => (
  <ThemeProvider theme={theme}>{children}</ThemeProvider>
);

ThemeWrapper.propTypes = {
  theme: PropTypes.any,
};

export default ThemeWrapper;
