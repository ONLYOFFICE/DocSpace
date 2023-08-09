import { createGlobalStyle } from "styled-components";

const StorybookGlobalStyles = createGlobalStyle`
  .sbdocs-content {
    direction: ltr;
  }

  .sbdocs-content .docs-story {
    direction: ${(props) => props.theme.interfaceDirection};
  }
`;

export default StorybookGlobalStyles;
