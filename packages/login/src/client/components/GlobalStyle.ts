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

  .with-background-pattern {
    background-image: url("/static/images/background.pattern.react.svg");
    background-repeat: no-repeat;
    background-attachment: fixed;
    background-size: 100% 100%;

    @media (max-width: 1024px) {
      background-size: cover;
    }

    @media (max-width: 428px) {
      background-image: none;
    }
  }
`;

export default GlobalStyle;
