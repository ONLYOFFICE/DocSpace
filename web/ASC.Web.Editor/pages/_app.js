import { createGlobalStyle } from "styled-components";
import { appWithTranslation } from "next-i18next";

const GlobalStyle = createGlobalStyle`
body {
    margin: 0;
}

html, body {
    height: 100%;
}
`;

const App = ({ Component, pageProps }) => {
  return (
    <>
      <GlobalStyle />
      <Component {...pageProps} />
    </>
  );
};

export default appWithTranslation(App);
