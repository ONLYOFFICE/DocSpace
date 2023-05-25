import React from "react";
import PropTypes from "prop-types";
import { ThemeProvider as Provider } from "styled-components";
import GlobalStyle from "../utils/globalStyles";

const ThemeProvider = (props) => {
  const { theme, currentColorScheme, children } = props;

  return (
    <Provider theme={{ ...theme, currentColorScheme }}>
      <GlobalStyle />
      {children}
    </Provider>
  );
};

ThemeProvider.propTypes = {
  /** Applies a theme to all children components */
  theme: PropTypes.object.isRequired,
  /** Applies a currentColorScheme to all children components */
  currentColorScheme: PropTypes.oneOfType([
    PropTypes.object.isRequired,
    PropTypes.bool.isRequired,
  ]),
  /** Child elements */
  children: PropTypes.any,
};

export default ThemeProvider;
