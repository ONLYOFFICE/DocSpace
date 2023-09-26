import { MINIMAL_VIEWPORTS } from "@storybook/addon-viewport";
import { withContexts } from "@storybook/addon-contexts/react";
import { contexts } from "./contexts/index";

import "../../../public/css/fonts.css";

export const parameters = {
  actions: { argTypesRegex: "^on[A-Z].*" },
  controls: { expanded: true },
  viewport: {
    viewports: MINIMAL_VIEWPORTS,
  },
};

export const decorators = [withContexts(contexts)];
