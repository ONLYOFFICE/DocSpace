import { MINIMAL_VIEWPORTS } from "@storybook/addon-viewport";
import { Base, Dark } from "../themes/index";
import "../../common/opensansoffline.scss";
import { ThemeWrapper } from "./contexts/theme-context";

const preview = {
  globalTypes: {
    theme: {
      description: "Global theme for components",
      defaultValue: "Light",
      toolbar: {
        title: "Theme",
        icon: "circlehollow",
        items: ["Light", "Dark"],
        dynamicTitle: true,
      },
    },
  },
  parameters: {
    backgrounds: { disable: true },
    actions: { argTypesRegex: "^on[A-Z].*" },
    controls: { expanded: true },
    viewport: {
      viewports: MINIMAL_VIEWPORTS,
    },
    previewTabs: {
      "storybook/docs/panel": {
        hidden: true,
      },
    },
  },
  decorators: [
    (Story, context) => {
      const theme = context.globals.theme;
      return (
        <ThemeWrapper theme={theme === "Dark" ? Dark : Base}>
          <Story />
        </ThemeWrapper>
      );
    },
  ],
};

export default preview;
