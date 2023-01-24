import { createGlobalStyle } from "styled-components";

interface IGlobalStyleProps {
  fonts?: string;
}

const GlobalStyle = createGlobalStyle`
  html,
  body {
    height: 100%;
    ${(props: IGlobalStyleProps) => props?.fonts}
    font-family: 'Open Sans', sans-serif, Arial;
    font-size: 13px;
  }

  #root {
    min-height: 100%;

    .pageLoader {
      position: fixed;
      left: calc(50% - 20px);
      top: 35%;
    }
  }
  body {
    margin: 0;
  }

  body.loading * {
    cursor: wait !important;
  }
`;

export default GlobalStyle;
