import { MINIMAL_VIEWPORTS } from "@storybook/addon-viewport";
import { withContexts } from "@storybook/addon-contexts/react";
import { contexts } from "./contexts/index";

import "../../common/opensansoffline.scss";

export const parameters = {
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
};

export const decorators = [withContexts(contexts)];
