import React from "react";
import PropTypes from "prop-types";
import { ThemeProvider as Provider } from "styled-components";
import GlobalStyle from "../utils/globalStyles";

const ThemeProvider = (props) => {
  const { theme, children } = props;

  return (
    <Provider theme={theme}>
      <GlobalStyle />
      {children}
    </Provider>
  );
};

ThemeProvider.propTypes = {
  /** Child elements */
  children: PropTypes.any,
  /** Applies a theme to all children components */
  theme: PropTypes.object.isRequired,
};

export default ThemeProvider;
