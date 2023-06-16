import React from "react";
import { DocsContainer as BaseContainer } from "@storybook/blocks";
import { themes } from "@storybook/theming";

export const DocsContainer = ({ children, context }) => {
  const modifiedDarkTheme = {
    ...themes.dark,
    appBg: "#333", // Replace 'new-color' with the desired background color
  };

  return (
    <BaseContainer
      context={context}
      theme={
        context.store?.globals.globals.theme === "Dark"
          ? modifiedDarkTheme
          : themes.light
      }
    >
      {children}
    </BaseContainer>
  );
};
