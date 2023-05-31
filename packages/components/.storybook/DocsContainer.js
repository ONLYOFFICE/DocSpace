import React from "react";
import { DocsContainer as BaseContainer } from "@storybook/blocks";
import { themes } from "@storybook/theming";

export const DocsContainer = ({ children, context }) => {
  return (
    <BaseContainer
      context={context}
      theme={
        context.store?.globals.globals.theme === "Dark"
          ? themes.dark
          : themes.light
      }
    >
      {children}
    </BaseContainer>
  );
};
