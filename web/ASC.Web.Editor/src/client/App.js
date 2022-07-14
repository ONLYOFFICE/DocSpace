import React from "react";
import { createGlobalStyle } from "styled-components";
//import Editor from "./Editor.js";

const GlobalStyle = createGlobalStyle`
  html,
  body {
    height: 100%;
  }

  #root {
    min-height: 100%;
    height: 100%;

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

export const App = () => {
  return (
    <>
      <GlobalStyle />
      <div id="editor"></div>
    </>
  );
};
