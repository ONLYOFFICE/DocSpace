import { MINIMAL_VIEWPORTS } from "@storybook/addon-viewport";
import { Base, Dark } from "../themes/index";
import "../../common/opensansoffline.scss";
import ThemeWrapper from "./globals/theme-wrapper";
import { DocsContainer } from "./DocsContainer";
import { useDarkMode } from "storybook-dark-mode";
import "../index";

import lightTheme from "./lightTheme";
import darkTheme from "./darkTheme";

import lightLogo from "./lightsmall.svg?url";
import darkLogo from "./darksmall.svg?url";

const preview = {
  parameters: {
    backgrounds: { disable: true },
    actions: { argTypesRegex: "^on[A-Z].*" },
    controls: { expanded: true },
    docs: {
      container: DocsContainer,
    },
    viewport: {
      viewports: MINIMAL_VIEWPORTS,
    },
    previewTabs: {
      "storybook/docs/panel": {
        hidden: true,
      },
    },
    darkMode: {
      current: "light",
      light: lightTheme,
      dark: darkTheme,
    },
  },
  decorators: [
    (Story) => {
      return (
        <ThemeWrapper theme={useDarkMode() ? Dark : Base}>
          <Story />
        </ThemeWrapper>
      );
    },
  ],
};

export default preview;
