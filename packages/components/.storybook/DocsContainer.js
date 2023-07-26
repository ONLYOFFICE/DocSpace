import React from "react";
import { DocsContainer as BaseContainer } from "@storybook/blocks";
import { useDarkMode } from "storybook-dark-mode";
import darkTheme from "./darkTheme";
import lightTheme from "./lightTheme";

export const DocsContainer = ({ children, context }) => {

  return (
    <BaseContainer
      context={context}
      theme={useDarkMode() ? darkTheme : lightTheme}
    >
      {children}
    </BaseContainer>
  );
};
