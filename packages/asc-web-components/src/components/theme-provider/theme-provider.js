import React from "react";
import PropTypes from "prop-types";
import { ThemeProvider as Provider } from "styled-components";
import GlobalStyle from "../../utils/globalStyles";

const ThemeProvider = props => {
  const { theme, children } = props;

  return (
    <Provider theme={theme}>
      <GlobalStyle />
      {children}
    </Provider>
  );
};

ThemeProvider.propTypes = {
  children: PropTypes.any,
  theme: PropTypes.object
};

export default ThemeProvider;
